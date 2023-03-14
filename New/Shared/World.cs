using System.Buffers;
using New.Engine;
using New.Shared.Components;
using OpenTK.Mathematics;

namespace New.Shared
{
  public class World
  {
    private static readonly Func<Entity, bool> _removed = obj => obj.Removed;

    private static readonly Action<Entity> _ifRemoved = obj => { ArrayPool<Entity.Component>.Shared.Return(obj.Components, true); };

    private readonly Dictionary<Vector2i, Chunk> _chunks;
    public readonly Vec<Entity> Objs = new();

    private Vector2i _prevCPos;

    public World()
    {
      _chunks = new Dictionary<Vector2i, Chunk>();
    }

    public void Update()
    {
      int k = 0;
      Span<Entity> objs = Objs.Items;
      for (int i = 0; i < Objs.Count; i++)
      {
        if (!objs[i].Updates || (objs[i].Pos - Fall.Player.Pos).Xz.LengthSquared > 72 * 256)
          continue;
        objs[i].Update();
        k++;
      }

      Fall.InView = k;

      Objs.RemoveAll(_removed, _ifRemoved);

      Vector2i chunkPos = Fall.Player.Pos.Xz.ToChunkPos();
      if (_prevCPos == chunkPos)
        return;

      for (int i = -12; i <= 12; i++)
      for (int j = -12; j <= 12; j++)
      {
        int i1 = i + chunkPos.X;
        int j1 = j + chunkPos.Y;
        if (!_chunks.ContainsKey((i1, j1)))
          _chunks[(i1, j1)] = new Chunk((i1, j1));
      }

      _prevCPos = chunkPos;
    }

    public void Render()
    {
      Vector2i chunkPos = Fall.Player.Pos.Xz.ToChunkPos();
      float lyaw = FloatPos.Get(Fall.Player).LerpedYaw;
      for (int i = -8; i <= 8; i++)
      for (int j = -8; j <= 8; j++)
      {
        int d = i * i + j * j;
        if (d > 81)
          continue;
        if (MathF.Abs(Math.WrapDegrees(Math.CalcAngle(j, i) - lyaw)) > 70 && d > 9)
          continue;
        _chunks[(i + chunkPos.X, j + chunkPos.Y)].Mesh.Render();
      }

      Span<Entity> objs = Objs.Items;
      for (int i = 0; i < Objs.Count; i++)
      {
        float d = (objs[i].Pos - Fall.Player.Pos).Xz.LengthSquared;
        if (d > 72 * 256)
          continue;
        if (MathF.Abs(Math.WrapDegrees(Math.CalcAngleXz(Fall.Player, objs[i]) - lyaw)) > 65 && d > 864)
          continue;
        objs[i].Render();
      }
    }

    public static float HeightAt(Vector2 vec)
    {
      return Chunk.HeightAt(vec);
    }
    
    public Chunk ChunkAt(Vector3 pos)
    {
      return _chunks[pos.Xz.ToChunkPos()];
    }

    public const float REACH = 24f;

    public HitResult Raycast(Vector3 eye, Vector3 dir, out float dist)
    {
      dist = REACH + 0.5f;
      Span<Entity> objs = Objs.Items;
      Vector3 b = new();
      Entity obj = null;
      HitResultType type = HitResultType.None;
      for (int i = 0; i < Objs.Count; i++)
      {
        if ((eye - objs[i].Pos).LengthSquared > 1024) continue;
        if (!objs[i].Has(CompType.Collision)) continue;

        if (!objs[i].Get<Collision>(CompType.Collision).RayCollides(objs[i].Pos, eye, dir, out float d))
          continue;

        if (!(d < dist) || d is > REACH or < 0) continue;

        dist = d;
        b = eye + dir * d;
        obj = objs[i];
        type = HitResultType.Entity;
      }

      Chunk chunk = null;
      Vector2i chunkPos = eye.Xz.ToChunkPos();
      for (int i = -1; i <= 1; i++)
      for (int j = -1; j <= 1; j++)
      {
        Chunk c = _chunks[(i + chunkPos.X, j + chunkPos.Y)];
        if (!c.Collision.RayCollides(Vector3.Zero, eye, dir, out float d))
          continue;
        
        if (d > dist || d is > REACH or < 0) continue;
        
        dist = d;
        b = eye + dir * d;
        chunk = c;
        type = HitResultType.Tile;
      }

      switch (type)
      {
        case HitResultType.Entity:
          return new HitResult(HitResultType.Entity, b.X, b.Y, b.Z, entity: obj);
        case HitResultType.Tile:
          return new HitResult(HitResultType.Tile, b.X, b.Y, b.Z, chunk: chunk);
        case HitResultType.None:
          break;
      }

      dist = 0;
      return new HitResult();
    }
  }

  public class Chunk
  {
    public readonly Mesh<P> Mesh;
    public readonly MeshCollision<P> Collision;
    private const int _quality = 4, _quality1 = _quality + 1, _tileSize = 16 / _quality;
    private const int _size = 16;
    private const float _div = 24f;

    public Chunk(Vector2i chunkPos)
    {
      Mesh = new Mesh<P>(DrawMode.TRIANGLE, RenderSystem.BASIC, true);
      Collision = new MeshCollision<P>(Mesh);

      Span<int> memo = stackalloc int[_quality1 * _quality1];
      memo.Fill(-1);

      Mesh.Begin();
      for (int i = 0; i < _quality; i++)
      for (int j = 0; j < _quality; j++)
      {
        int i1, i2, i3, i4;
        int ti = i * _tileSize + chunkPos.X * _size, tj = j * _tileSize + chunkPos.Y * _size;
        if ((i1 = memo[i * _quality1 + j]) == -1)
          i1 = memo[i * _quality1 + j] =
            Mesh.Put(new(ti, Noise(ti, tj), tj)).Next();

        if ((i2 = memo[(i + 1) * _quality1 + j]) == -1)
          i2 = memo[(i + 1) * _quality1 + j] =
            Mesh.Put(new(ti + _tileSize, Noise(ti + _tileSize, tj), tj)).Next();

        if ((i3 = memo[(i + 1) * _quality1 + j + 1]) == -1)
          i3 = memo[(i + 1) * _quality1 + j + 1] =
            Mesh.Put(new(ti + _tileSize, Noise(ti + _tileSize, tj + _tileSize), tj + _tileSize)).Next();

        if ((i4 = memo[i * _quality1 + j + 1]) == -1)
          i4 = memo[i * _quality1 + j + 1] =
            Mesh.Put(new(ti, Noise(ti, tj + _tileSize), tj + _tileSize)).Next();
        Mesh.Quad(i4, i3, i2, i1);
      }

      Mesh.End();
    }

    private static float Noise(int x, int y)
    {
      return SimplexNoise.Noise.CalcPixel2D(x, y, 0.01f) / _div;
    }

    public static float HeightAt(Vector2 vec)
    {
      int x1 = (int)(vec.X - vec.X % _tileSize);
      int y1 = (int)(vec.Y - vec.Y % _tileSize);

      float v00 = Noise(x1, y1);
      float v10 = Noise(x1 + _tileSize, y1);
      float v01 = Noise(x1, y1 + _tileSize);
      float v11 = Noise(x1 + _tileSize, y1 + _tileSize);
      float x = (vec.X - x1) / _tileSize;
      float y = (vec.Y - y1) / _tileSize;

      // bilinear interpolation
      return (1 - x) * (1 - y) * v00 + x * (1 - y) * v10 + (1 - x) * y * v01 + x * y * v11;
    }
  }
}