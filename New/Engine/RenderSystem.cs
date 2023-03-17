using New.Shared;
using New.Shared.Components;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New.Engine
{
  public static class RenderSystem
  {
    public const float THRESHOLD = 0.013f;
    private const float _depthThreshold = 0.8f;

    public static readonly Shader BASIC = new("basic", "basic");
    public static readonly Shader BASIC_INSTANCED = new("basicinstanced", "basic");
    private static readonly Shader _john = new("john", "john");
    private static readonly Shader _lines = new("lines", "lines", "lines");
    private static readonly Shader _pixel = new("postprocess", "pixelate");
    private static readonly Shader _fxaa = new("fxaa", "fxaa");
    private static readonly Shader _outline = new("postprocess", "outline");
    private static readonly Shader _blobs = new("blobs", "blobs");
    private static readonly Shader _outlineWatercolor = new("sobel", "outlinewatercolor");
    private static readonly Shader _outlineCombine = new("sobel", "outlinecombine");
    private static readonly Shader _blur = new("sobel", "blur");
    private static readonly Shader _quads = new("quads", "quads");

    private static Matrix4 _proj;
    private static Matrix4 _look;
    private static readonly Matrix4[] _model = new Matrix4[7];
    private static int _modelIdx;

    public static readonly Mesh<PCT> MESH = new(DrawMode.TRIANGLE, _john, false);
    public static readonly Mesh<PC> QUADS = new(DrawMode.TRIANGLE, _quads, false);
    public static readonly Mesh<PWC> LINE = new(DrawMode.LINE, _lines, false);

    private static readonly Mesh<P> _post = new(DrawMode.TRIANGLE, null, false);

    public static readonly Image2d RECT = Image2d.FromFile("Resource/Texture/rect.png");

    public static readonly Fbo FRAME0 = new(1, 1, true);
    public static readonly Fbo FRAME1 = new(1, 1, true);
    public static readonly Fbo FRAME2 = new(1, 1, true);
    public static bool Rendering3d;
    public static bool Culling = true;
    public static bool RenderingRed;
    private static Entity _camera;

    static RenderSystem()
    {
      Array.Fill(_model, Matrix4.Identity);
    }

    public static ref Matrix4 Model => ref _model[_modelIdx];

    public static Vector2i Size => Fall.Instance.Size;

    public static void Resize()
    {
      _post.Begin();
      _post.Quad(
        _post.Put(new P(-1, -1)).Next(),
        _post.Put(new P(1, -1)).Next(),
        _post.Put(new P(1, 1)).Next(),
        _post.Put(new P(-1, 1)).Next()
      );
      _post.End();
    }

    public static void Push()
    {
      _model[_modelIdx + 1] = Model;
      _modelIdx++;
    }

    public static void Pop()
    {
      _modelIdx--;
    }

    public static void Translate(Vector3 vec)
    {
      Model.Translate(vec);
    }

    public static void Translate(float x, float y, float z)
    {
      Model.Translate(x, y, z);
    }

    public static void Rotate(float angle, float x, float y, float z)
    {
      Model.Rotate(angle, x, y, z);
    }

    public static void Rotate(float angle, Vector3 v)
    {
      Model.Rotate(angle, v.X, v.Y, v.Z);
    }


    public static void Scale(float scale)
    {
      Model.Scale(scale);
    }

    public static Vector4 ToVector4(this Color4 color)
    {
      return (color.R, color.G, color.B, color.A);
    }

    public static Vector4 ToVector4(this uint val)
    {
      return (((val >> 16) & 0xff) / 255f, ((val >> 8) & 0xff) / 255f, (val & 0xff) / 255f,
        ((val >> 24) & 0xff) / 255f);
    }

    public static void SetDefaults(this Shader shader)
    {
      shader.SetInt("_renderingRed", RenderingRed ? 1 : 0);
      shader.SetInt("_rendering3d", Rendering3d ? 1 : 0);
      shader.SetInt("doLighting", 0);
      shader.SetVector2("_screenSize", (Size.X, Size.Y));
      shader.SetVector3("lightPos", (_camera.X + 5, _camera.Y + 12, _camera.Z + 5));
      shader.SetMatrix4("_proj", _proj);
      shader.SetMatrix4("_look", _look);
      shader.SetFloat("_time", (float)GLFW.GetTime() % (MathF.PI * 2f));
      shader.SetVector2("_radius", (2, 2));
    }

    public static void RenderPixelation(float pixWidth, float pixHeight)
    {
      FRAME1.ClearColor();
      FRAME1.ClearDepth();
      FRAME1.Bind();
      _pixel.Bind();
      FRAME0.BindColor(TextureUnit.Texture0);
      _pixel.SetInt("_tex0", 0);
      _pixel.SetVector2("_screenSize", (Size.X, Size.Y));
      _pixel.SetVector2("_pixSize", (pixWidth, pixHeight));
      _post.Render();
      FRAME1.Blit(FRAME0.Handle);
      Shader.Unbind();
    }

    public static void RenderFxaa(Fbo fbo)
    {
      fbo.Blit(FRAME1.Handle);
      FRAME1.Bind();
      _fxaa.Bind();
      fbo.BindColor(TextureUnit.Texture0);
      _fxaa.SetInt("_tex0", 0);
      _fxaa.SetFloat("_spanMax", 8);
      _fxaa.SetFloat("_reduceMul", 0.125f);
      _fxaa.SetFloat("_subPixelShift", 0.25f);
      _fxaa.SetVector2("_screenSize", (Size.X, Size.Y));
      _post.Render();
      Shader.Unbind();
      FRAME1.Blit(fbo.Handle);
    }

    public static void RenderOutline()
    {
      FRAME0.ClearColor();
      FRAME0.ClearDepth();
      FRAME0.Bind();
      _outline.Bind();
      FRAME1.BindColor(TextureUnit.Texture0);
      _outline.SetInt("_tex0", 0);
      FRAME1.BindDepth(TextureUnit.Texture1);
      _outline.SetInt("_tex1", 1);
      _outline.SetInt("_glow", 0);
      _outline.SetFloat("_width", 1f);
      _outline.SetFloat("_threshold", THRESHOLD);
      _outline.SetFloat("_depthThreshold", _depthThreshold);
      _outline.SetVector2("_screenSize", (Size.X, Size.Y));
      _outline.SetVector4("_outlineColor", Fall.PINK0.ToVector4());
      _outline.SetVector4("_otherColor", Color4.White.ToVector4());
      _post.Render();
      Shader.Unbind();
    }

    public static void RenderOutlineWaterColor()
    {
      FRAME1.Clear();
      _outlineWatercolor.Bind();
      _outlineWatercolor.SetFloat("_lumaRamp", 16f);
      _outlineWatercolor.SetVector2("_screenSize", (Size.X, Size.Y));
      FRAME1.Bind();
      FRAME0.BindColor(0);
      _outlineWatercolor.SetInt("_tex0", 0);
      _post.Render();
      FRAME1.Blit(FRAME0.Handle);
    }

    public static void RenderBokeh(float radius)
    {
      FRAME1.Clear();
      FRAME2.Clear();

      _blobs.Bind();
      _blobs.SetFloat("_radius", radius);
      _blobs.SetVector2("_screenSize", (Size.X, Size.Y));
      FRAME1.Bind();
      FRAME0.BindColor(0);
      _blobs.SetInt("_tex0", 0);
      _post.Render();

      _outlineWatercolor.Bind();
      _outlineWatercolor.SetFloat("_lumaRamp", 16f);
      _outlineWatercolor.SetVector2("_screenSize", (Size.X, Size.Y));
      FRAME0.Bind();
      FRAME1.BindColor(0);
      _outlineWatercolor.SetInt("_tex0", 0);
      _post.Render();

      _blur.Bind();
      _blur.SetVector2("_blurDir", (0f, 0.8f));
      _blur.SetVector2("_screenSize", (Size.X, Size.Y));
      _blur.SetFloat("_radius", radius * 2);
      FRAME2.Bind();
      FRAME0.BindColor(0);
      _blur.SetInt("_tex0", 0);
      _post.Render();

      _blur.Bind();
      _blur.SetVector2("_blurDir", (0.8f, 0f));
      _blur.SetVector2("_screenSize", (Size.X, Size.Y));
      _blur.SetFloat("_radius", radius * 2);
      FRAME0.Bind();
      FRAME2.BindColor(0);
      _blur.SetInt("_tex0", 0);
      _post.Render();

      _outlineCombine.Bind();
      _outlineCombine.SetVector2("_screenSize", (Size.X, Size.Y));
      FRAME2.Bind();
      FRAME1.BindColor(0);
      _outlineCombine.SetInt("_tex0", 0);
      FRAME0.BindColor(1);
      _outlineCombine.SetInt("_tex1", 1);
      _post.Render();

      FRAME2.Blit(FRAME0.Handle);
    }

    public static void Line(float x1, float y1, float x2, float y2, float width, uint color)
    {
      LINE.Line(
        LINE.Put(new(x1, y1, 0, width, color)).Next(),
        LINE.Put(new(x2, y2, 0, width, color)).Next()
      );
    }

    public static void Line(Vector3 v1, Vector3 v2, float width, uint color)
    {
      LINE.Line(
        LINE.Put(new(v1, width, color)).Next(),
        LINE.Put(new(v2, width, color)).Next()
      );
    }

    public static void UpdateProjection()
    {
      if (Rendering3d)
      {
        Matrix4.CreatePerspectiveFieldOfView(Camera.FOV, Size.X / (float)Size.Y, Camera.NEAR, Camera.FAR,
          out _proj);
        return;
      }

      Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1000, 3000, out _proj);
    }

    public static void UpdateLookAt(Entity cameraObj, bool rendering3d = true)
    {
      _camera = cameraObj;
      Rendering3d = rendering3d;
      if (!Rendering3d)
      {
        _look = Matrix4.Identity;
        return;
      }

      Camera comp = Camera.Get(cameraObj);
      _look = comp.GetCameraMatrix();
    }

    public static Vector3 Project(Vector3 worldPos)
    {
      Vector4 clip = new Vector4(worldPos, 1) * _look * _proj;
      float winX = (clip.X / clip.Z + 1) / 2f * Fall.Instance.Size.X;
      float winY = (1 - clip.Y / clip.Z) / 2f * Fall.Instance.Size.Y;
      return (winX, winY, clip.Z);
    }
  }
}