using New.Shared.Components;
using OpenTK.Mathematics;

namespace New.Shared.Worlds;

public sealed class World
{
  private static readonly Func<Entity, bool> _removed = obj => obj.Removed;
  private static readonly Action<Entity> _ifRemoved = obj => obj.Die();

  private readonly Dictionary<Vector2i, Chunk> _chunks;
  public readonly Vec<Entity> Objs = new(16384);
  public readonly Vec<Entity> ObjsToAdd = new(1024);

  private Vector2i _prevCPos = new Vector2i(1000);

  public World()
  {
    _chunks = new Dictionary<Vector2i, Chunk>();
  }
  
  public void Add(Entity entity)
  {
    if (Objs.CanAddWithoutResize())
    {
      Objs.Add(entity);
      return;
    }
    ObjsToAdd.Add(entity);
  }

  public void Update()
  {
    int k = 0;
    Span<Entity> s = Objs.Items;
    for (int i = 0; i < Objs.Count; i++)
    {
      if (s[i] == null) continue;
      float dx = s[i].X - Fall.Player.X, dz = s[i].Z - Fall.Player.Z;
      float d = dx * dx + dz * dz;
      if (!s[i].Updates || d > 18432)
        continue;
      s[i].Update();
      k++;
    }

    if (ObjsToAdd.Count != 0)
    {
      Objs.AddRange(ObjsToAdd);
      ObjsToAdd.Clear();
    }
    Fall.InView = k;

    Objs.RemoveAll(_removed, _ifRemoved);

    Vector2i chunkPos = Fall.Player.ChunkPos;
    if (_prevCPos == chunkPos)
      return;

    for (int i = -12; i <= 12; i++)
    for (int j = -12; j <= 12; j++)
    {
      int i1 = i + chunkPos.X;
      int j1 = j + chunkPos.Y;
      if (!_chunks.ContainsKey((i1, j1))) _chunks[(i1, j1)] = new Chunk((i1, j1));
    }

    _prevCPos = chunkPos;
  }

  public void Render()
  {
    Vector2i chunkPos = Fall.Player.ChunkPos;
    float lyaw = Fall.Player.LerpedYaw;
    for (int i = -8; i <= 8; i++)
    for (int j = -8; j <= 8; j++)
    {
      int d = i * i + j * j;
      if (d > 81)
        continue;
      if (MathF.Abs(Maths.wrap_degrees(Maths.calc_angle(j, i) - lyaw)) > 75 && d > 9)
        continue;
      _chunks[(chunkPos.X + i, chunkPos.Y + j)].Draw();
    }
    
    Span<Entity> objs = Objs.Items;
    for (int i = 0; i < Objs.Count; i++)
    {
      if (objs[i] == null) continue;
      float dx = objs[i].X - Fall.Player.X, dz = objs[i].Z - Fall.Player.Z;
      float d = dx * dx + dz * dz;
      if (d > 18432 || (MathF.Abs(Maths.wrap_degrees(Maths.calc_angle_xz(Fall.Player, objs[i]) - lyaw)) > 65 && d > 864))
        continue;

      objs[i].Render();
    }
  }

  public static float HeightAt(Vector2 vec)
  {
    return Chunk.HeightAt(vec);
  }

  public static float HeightAt(float x, float z)
  {
    return Chunk.HeightAt((x, z));
  }

  public const float REACH = 24f;
  public const float REACH_SQ = REACH * REACH;

  public HitResult Raycast(Vector3 eye, Vector3 dir)
  {
    float dist = REACH + 0.5f;
    Span<Entity> objs = Objs.Items;
    Vector3 b = new();
    Entity obj = null;
    HitResultType type = HitResultType.None;
    for (int i = 0; i < Objs.Count; i++)
    {
      if (objs[i] == null) continue;
      float dx = objs[i].X - eye.X, dz = objs[i].Z - eye.Z;
      float di = dx * dx + dz * dz;
      if (di > REACH_SQ) continue;
      if (!objs[i].Has(CompType.Collision)) continue;

      if (!objs[i].Get<Collision>(CompType.Collision).ray_collides(objs[i].Pos, eye, dir, out float d))
        continue;

      if (!(d < dist) || d is > REACH or < 0) continue;

      dist = d;
      b = eye + dir * d;
      obj = objs[i];
      type = HitResultType.Entity;
    }

    Chunk chunk = null;
    Vector2i chunkPos = eye.to_chunk_pos();
    for (int i = -1; i <= 1; i++)
    for (int j = -1; j <= 1; j++)
    {
      if (!_chunks.TryGetValue((i + chunkPos.X, j + chunkPos.Y), out Chunk c)) continue;
      if (!c.Collision.ray_collides(Vector3.Zero, eye, dir, out float d))
        continue;

      if (d > dist || d is > REACH or < 0) continue;

      dist = d;
      b = eye + dir * d;
      chunk = c;
      type = HitResultType.Tile;
    }

    return type switch
    {
      HitResultType.Entity => new HitResult(HitResultType.Entity, dist, b.X, b.Y, b.Z, entity: obj),
      HitResultType.Tile => new HitResult(HitResultType.Tile, dist, b.X, b.Y, b.Z, chunk: chunk),
      HitResultType.None => new HitResult(),
      _ => new HitResult()
    };
  }
}