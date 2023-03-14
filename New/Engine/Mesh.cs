using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace New.Engine
{
  public class Mesh<T> where T : struct, IPos3d
  {
    private readonly DrawMode _drawMode;
    private readonly Ibo _ibo;
    private readonly Shader _shader;
    private readonly bool _static;
    private readonly Vao _vao;
    private readonly Vbo<T> _vbo;
    private bool _building;
    private int _index;
    private int _tris;
    private uint _vertex;

    private const float _oneOver255 = 1.0f / 255.0f;

    public Mesh(DrawMode drawMode, Shader shader, bool @static)
    {
      _drawMode = drawMode;
      _shader = shader;
      int stride = VertexTypes.Layouts[typeof(T)].Sum(attrib => (int)attrib * sizeof(float));
      _vbo = new Vbo<T>(stride * drawMode.Size * sizeof(float), @static);
      _vbo.Bind();
      _ibo = new Ibo(drawMode.Size * 128 * sizeof(float), @static);
      _ibo.Bind();
      _vao = new Vao(VertexTypes.Layouts[typeof(T)]);
      Vbo<T>.Unbind();
      Ibo.Unbind();
      Vao.Unbind();
      _static = @static;
    }

    public uint Next()
    {
      return _vertex++;
    }

    public Mesh<T> Put(T vertex)
    {
      _vbo.Put(vertex);
      return this;
    }

    public void Single(uint p0)
    {
      _ibo.Put(p0);
      _index++;
    }

    public void Line(uint p0, uint p1)
    {
      _ibo.Put(p0);
      _ibo.Put(p1);
      _index += 2;
    }

    public void Tri(uint p0, uint p1, uint p2)
    {
      _ibo.Put(p0);
      _ibo.Put(p1);
      _ibo.Put(p2);
      _index += 3;
      _tris++;
    }

    public void Quad(uint p0, uint p1, uint p2, uint p3)
    {
      _ibo.Put(p0);
      _ibo.Put(p1);
      _ibo.Put(p2);
      _ibo.Put(p2);
      _ibo.Put(p3);
      _ibo.Put(p0);
      _index += 6;
      _tris += 2;
    }

    public void Begin()
    {
      if (_building) throw new Exception("Already building");
      if (!_static)
      {
        _vbo.Clear();
        _ibo.Clear();
        _tris = 0;
      }

      _vertex = 0;
      _index = 0;
      _building = true;
    }

    public void End()
    {
      if (!_building) throw new Exception("Not building");

      if (_index > 0)
      {
        _vbo.Upload();
        _ibo.Upload();
      }

      if (_static)
      {
        _vbo.Clear();
        _ibo.Clear();
      }

      _building = false;
    }

    public void Render()
    {
      if (_building) End();

      if (_index <= 0) return;
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

      _shader?.Bind();
      _shader?.SetDefaults();
      _vao.Bind();
      _ibo.Bind();
      _vbo.Bind();
      GL.DrawElements(_drawMode.AsGl(), _index, DrawElementsType.UnsignedInt, 0);
      Ibo.Unbind();
      Vbo<T>.Unbind();
      Vao.Unbind();
      GlStateManager.RestoreState();
      Fall.Tris += _tris;
    }

    public void RenderInstanced(int numInstances)
    {
      if (_building) End();

      if (_index <= 0 || numInstances <= 0) return;
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

      _shader?.Bind();
      _shader?.SetDefaults();
      _vao.Bind();
      _ibo.Bind();
      _vbo.Bind();
      GL.DrawElementsInstanced(_drawMode.AsGlPrim(), _index, DrawElementsType.UnsignedInt, IntPtr.Zero, numInstances);
      Ibo.Unbind();
      Vbo<T>.Unbind();
      Vao.Unbind();
      GlStateManager.RestoreState();
      Fall.Tris += _tris * numInstances;
    }

    public ref T ClosestVertex(Vector3 pos)
    {
      int closest = 0;
      float dist = (pos - _vbo.Vertices[0].Pos).LengthSquared;
      for (int i = 1; i < _vbo.Vertices.Length; i++)
      {
        float d = (pos - _vbo.Vertices[i].Pos).LengthSquared;
        if (!(d < dist)) continue;
        
        dist = d;
        closest = i;
      }
      
      return ref _vbo.Vertices[closest];
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
}