using New.Engine;
using New.Shared.Components;
using OpenTK.Mathematics;

namespace New.Shared.Worlds.World
{
  public class Chunk
  {
    public readonly Mesh<PC> Mesh;
    public readonly MeshCollision<PC> Collision;
    public readonly Vector2i Pos;

    public const int SHIFT = 4;
    private const int _quality = 8, _quality1 = _quality + 1, _tileSize = 16 / _quality;
    public static readonly int SIZE = 16;
    private static readonly Mesh<PC> _waterMesh = new Mesh<PC>(DrawMode.TRIANGLE, RenderSystem.BASIC_INSTANCED, true, 6);
    private static readonly Ubo _ubo = new Ubo(RenderSystem.BASIC_INSTANCED, "_instanceInfo", "_translation");
    private static readonly Vec<Vector4> _translations = new Vec<Vector4>(64);

    static Chunk()
    {
      _waterMesh.Begin();
      _waterMesh.Quad(
        _waterMesh.Put(new PC(0, 0, 0, 0xff000056)).Next(),
        _waterMesh.Put(new PC(0, 0, SIZE, 0xff000056)).Next(),
        _waterMesh.Put(new PC(SIZE, 0, SIZE, 0xff000056)).Next(),
        _waterMesh.Put(new PC(SIZE, 0, 0, 0xff000056)).Next()
      );
      _waterMesh.End();
    }

    public Chunk(Vector2i chunkPos)
    {
      Pos = chunkPos;
      Mesh = new Mesh<PC>(DrawMode.TRIANGLE, RenderSystem.BASIC, true, _quality * _quality);
      Collision = new MeshCollision<PC>(Mesh);

      Span<int> memo = stackalloc int[_quality1 * _quality1];
      memo.Fill(-1);

      Mesh.Begin();
      for (int i = 0; i < _quality; i++)
      for (int j = 0; j < _quality; j++)
      {
        int i1, i2, i3, i4;
        if ((i1 = memo[i * _quality1 + j]) == -1)
          i1 = memo[i * _quality1 + j] = Mesh.Put(GetVertex(i, j)).Next();

        if ((i2 = memo[(i + 1) * _quality1 + j]) == -1)
          i2 = memo[(i + 1) * _quality1 + j] = Mesh.Put(GetVertex(i + 1, j)).Next();

        if ((i3 = memo[(i + 1) * _quality1 + j + 1]) == -1)
          i3 = memo[(i + 1) * _quality1 + j + 1] = Mesh.Put(GetVertex(i + 1, j + 1)).Next();

        if ((i4 = memo[i * _quality1 + j + 1]) == -1)
          i4 = memo[i * _quality1 + j + 1] = Mesh.Put(GetVertex(i, j + 1)).Next();
        Mesh.Quad(i4, i3, i2, i1);
      }

      Mesh.End();
    }

    public void Draw()
    {
      Mesh.Render();
      _translations.Add(new Vector4(Pos.X * SIZE, 0, Pos.Y * SIZE, 1));
    }

    public static void AfterDraw()
    {
      Vector4[] models = _translations.Items;

      int count = Math.Min(1024, _translations.Count);

      RenderSystem.BASIC_INSTANCED.Bind();
      _ubo.PutAll(ref models, count, 0);
      _ubo.BindTo(0);
      _waterMesh.RenderInstanced(count);

      Shader.Unbind();
      _translations.Clear();
    }

    private PC GetVertex(int x, int z)
    {
      return new PC(x * _tileSize + Pos.X * SIZE, Noise(x * _tileSize + Pos.X * SIZE, z * _tileSize + Pos.Y * SIZE),
        z * _tileSize + Pos.Y * SIZE, 0xffffffff);
    }

    private static float Noise(int x, int y)
    {
      return Biome.Variance(x, y) * 3 + Biome.Height(x, y) * 20;
    }

    public static float HeightAt(Vector2 vec)
    {
      int x1 = (int)MathF.Floor(vec.X);
      int y1 = (int)MathF.Floor(vec.Y);

      float v00 = Noise(x1, y1);
      float v10 = Noise(x1 + 1, y1);
      float v01 = Noise(x1, y1 + 1);
      float v11 = Noise(x1 + 1, y1 + 1);
      float x = vec.X - x1;
      float y = vec.Y - y1;

      // bilinear interpolation
      return (1 - x) * (1 - y) * v00 + x * (1 - y) * v10 + (1 - x) * y * v01 + x * y * v11;
    }
  }
}