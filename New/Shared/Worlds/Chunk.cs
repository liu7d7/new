using New.Engine;
using New.Shared.Components;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace New.Shared.Worlds;

public sealed class Chunk
{
  public readonly Mesh<PC> Mesh;
  public readonly MeshCollision<PC> Collision;
  public readonly Vector2i Pos;
  private bool _water;

  public const int SHIFT = 4;
  public static readonly int SIZE = 16;
  private const int _quality = 8, _quality1 = _quality + 1, _tileSize = 16 / _quality;

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
        i1 = memo[i * _quality1 + j] = Mesh.Put(get_vertex(i, j));

      if ((i2 = memo[(i + 1) * _quality1 + j]) == -1)
        i2 = memo[(i + 1) * _quality1 + j] = Mesh.Put(get_vertex(i + 1, j));

      if ((i3 = memo[(i + 1) * _quality1 + j + 1]) == -1)
        i3 = memo[(i + 1) * _quality1 + j + 1] = Mesh.Put(get_vertex(i + 1, j + 1));

      if ((i4 = memo[i * _quality1 + j + 1]) == -1)
        i4 = memo[i * _quality1 + j + 1] = Mesh.Put(get_vertex(i, j + 1));
      Mesh.Quad(i4, i3, i2, i1);
    }

    Mesh.End();
  }

  public void Draw()
  {
    Mesh.Render();
    if (!_water) return;
    
    RenderSystem.WATER.Bind();
    RenderSystem.NOISE.Bind(TextureUnit.Texture0);
    RenderSystem.WATER.SetInt("_noise", 0);
    RenderSystem.DISTORTION.Bind(TextureUnit.Texture1);
    RenderSystem.WATER.SetInt("_distortion", 1);
    Mesh.Render(RenderSystem.WATER);
  }

  private PC get_vertex(int x, int z)
  {
    float y = Noise(x * _tileSize + Pos.X * SIZE, z * _tileSize + Pos.Y * SIZE);
    _water = _water || y < 0;
    return new PC(x * _tileSize + Pos.X * SIZE, y, z * _tileSize + Pos.Y * SIZE, 0xffffffff);
  }

  private static float Noise(int x, int y)
  {
    return Biome.Variance(x, y) * 12 + Biome.Height(x, y) * 40 - 6;
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