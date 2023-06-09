using New.Shared;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace New.Engine;

public sealed class Mesh<T> : IPosProvider where T : struct, IPos3d
{
  private readonly DrawMode _drawMode;
  private readonly Ibo _ibo;
  private readonly Shader _shader;
  private readonly bool _static;
  private readonly Vao _vao;
  public readonly Vbo<T> Vbo;
  private bool _building;
  private int _numIndices;
  private int _tris;
  private int _numVertices;
  private bool _dirty;
  public Vec<(int, int, int)> Tris = new();

  public Mesh(DrawMode drawMode, Shader shader, bool @static, int initialCap = 256)
  {
    _drawMode = drawMode;
    _shader = shader;
    Vbo = new Vbo<T>(initialCap, @static);
    Vbo.Bind();
    _ibo = new Ibo(initialCap * 6, @static);
    _ibo.Bind();
    _vao = new Vao(VertexTypes.Layouts[typeof(T)]);
    Vbo<T>.Unbind();
    Ibo.Unbind();
    Vao.Unbind();
    _static = @static;
  }

  public int Put(T vertex)
  {
    Vbo.Put(vertex);
    return _numVertices++;
  }

  public void Single(int p0)
  {
    _ibo.Put(p0);
    _numIndices++;
  }

  public void Line(int p0, int p1)
  {
    _ibo.Put(p0);
    _ibo.Put(p1);
    _numIndices += 2;
  }

  public void Tri(int p0, int p1, int p2)
  {
    _ibo.Put(p0);
    _ibo.Put(p1);
    _ibo.Put(p2);
    Tris.Add((p0, p1, p2));
    _numIndices += 3;
    _tris++;
  }

  public void Quad(int p0, int p1, int p2, int p3)
  {
    _ibo.Put(p0);
    _ibo.Put(p1);
    _ibo.Put(p2);
    _ibo.Put(p2);
    _ibo.Put(p3);
    _ibo.Put(p0);
    Tris.Add((p0, p1, p2));
    Tris.Add((p2, p3, p0));
    _numIndices += 6;
    _tris += 2;
  }

  public void Begin()
  {
    if (_building) throw new Exception("Already building");
    if (!_static)
    {
      Vbo.Clear();
      _ibo.Clear();
      Tris.Clear();
      _tris = 0;
    }

    _numVertices = 0;
    _numIndices = 0;
    _building = true;
  }

  public void End()
  {
    if (!_building && !_dirty) throw new Exception("Not building");

    if (_numIndices > 0)
    {
      Vbo.Upload();
      _ibo.Upload();
    }

    _building = false;
    _dirty = false;
  }

  public void Render(Shader s = null)
  {
    if (_building) End();

    if (_numIndices <= 0) return;

    GlStateManager.SaveState();
    GlStateManager.EnableBlend();
    if (RenderSystem.Rendering3d)
    {
      GlStateManager.EnableDepth();
      if (RenderSystem.Culling)
      {
        GlStateManager.EnableCull();
      }
      else
      {
        GlStateManager.DisableCull();
      }
    }
    else
    {
      GlStateManager.DisableDepth();
      GlStateManager.DisableCull();
    }
    
    (s ?? _shader)?.Bind();
    (s ?? _shader)?.SetDefaults();
    _vao.Bind();
    _ibo.Bind();
    Vbo.Bind();
    GL.DrawElements(_drawMode.AsGl(), _numIndices, DrawElementsType.UnsignedInt, 0);
    Ibo.Unbind();
    Vbo<T>.Unbind();
    Vao.Unbind();
    GlStateManager.RestoreState();
    Fall.Tris += _tris;
  }

  public void RenderInstanced(int numInstances, Shader s = null)
  {
    if (_building) End();

    if (_numIndices <= 0 || numInstances <= 0) return;
    GlStateManager.SaveState();
    GlStateManager.EnableBlend();
    if (RenderSystem.Rendering3d)
    {
      GlStateManager.EnableDepth();
      if (RenderSystem.Culling)
      {
        GlStateManager.EnableCull();
      }
      else
      {
        GlStateManager.DisableCull();
      }
    }
    else
    {
      GlStateManager.DisableDepth();
      GlStateManager.DisableCull();
    }

    (s ?? _shader)?.Bind();
    (s ?? _shader)?.SetDefaults();
    _vao.Bind();
    _ibo.Bind();
    Vbo.Bind();
    GL.DrawElementsInstanced(_drawMode.AsGlPrim(), _numIndices, DrawElementsType.UnsignedInt, IntPtr.Zero, numInstances);
    Ibo.Unbind();
    Vbo<T>.Unbind();
    Vao.Unbind();
    GlStateManager.RestoreState();
    Fall.Tris += _tris * numInstances;
  }

  public (int, Vector3) closest_vertex(Vector3 pos)
  {
    if (Vbo.Vertices == null)
    {
      throw new Exception("No vertices");
    }

    int closest = 0;
    float dist = (pos - Vbo.Vertices[0].Pos).LengthSquared;
    for (int i = 1; i < Vbo.Vertices.Length; i++)
    {
      float d = (pos - Vbo.Vertices[i].Pos).LengthSquared;
      if (d > dist) continue;

      dist = d;
      closest = i;
    }

    return (closest, Vbo.Vertices[closest].Pos);
  }

  public void set_pos(int index, Vector3 pos)
  {
    if (Vbo.Vertices == null)
    {
      throw new Exception("No vertices");
    }

    Vbo.Vertices[index].Pos = pos;
    _dirty = true;
  }
}

public sealed class DrawMode
{
  public static readonly DrawMode LINE = new(2, BeginMode.Lines, PrimitiveType.Lines);
  public static readonly DrawMode TRIANGLE = new(3, BeginMode.Triangles, PrimitiveType.Triangles);
  public static readonly DrawMode TRIANGLE_STRIP = new(2, BeginMode.TriangleStrip, PrimitiveType.TriangleStrip);
  private readonly BeginMode _mode;
  private readonly PrimitiveType _prim;
  public readonly int Size;

  private DrawMode(int size, BeginMode mode, PrimitiveType prim)
  {
    Size = size;
    _mode = mode;
    _prim = prim;
  }

  public override bool Equals(object obj)
  {
    if (obj is DrawMode mode) return _mode == mode._mode;

    return false;
  }

  public override int GetHashCode()
  {
    return _mode.GetHashCode();
  }

  public BeginMode AsGl()
  {
    return _mode;
  }

  public PrimitiveType AsGlPrim()
  {
    return _prim;
  }
}