using System.Drawing;
using System.Runtime.InteropServices;
using New.Engine;
using New.Shared;
using New.Shared.Components;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New
{
  public class Fall : GameWindow
  {
    public const uint PINK0 = 0xffff34b4;
    public static Fall Instance;
    public static Entity Player;
    public static World World;
    public static float MouseDx, MouseDy, MouseX, MouseY;
    public static uint Ticks;
    public static uint Frames;
    public static int InView;
    public static int Tris;
    public static bool FirstPerson;
    public static HitResult HitResult = new HitResult();

    private static readonly DebugProc _debugProcCallback = DebugCallback;
    private static readonly Keys[] _keys = (Keys[])Enum.GetValues(typeof(Keys));
    private static readonly HashSet<Keys> _keysDown = new();
    private static GCHandle _debugProcCallbackHandle;
    private static float _lastTick;
    private readonly Color4 _backgroundColor = Colors.NextColor();
    private readonly RollingAvg _mspf = new(120);
    private int _lastInquiry;
    private int _memUsage;
    private bool _outline = true;

    public static float Now => (float)GLFW.GetTime() * 1000f;
    public static float TickDelta => (Now - _lastTick) / 50f;

    public Fall(GameWindowSettings windowSettings, NativeWindowSettings nativeWindowSettings) : base(windowSettings,
      nativeWindowSettings)
    {
      Instance = this;
      GL.Enable(EnableCap.Multisample);
      RenderSystem.Resize();
      CreateWorld();

      _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);
      GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    private static void DebugCallback(DebugSource source, DebugType type, int id,
      DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
      string messageString = Marshal.PtrToStringAnsi(message, length);
      Console.WriteLine($"{severity} {type} | {messageString}");

      if (type == DebugType.DebugTypeError)
        throw new Exception(messageString);
    }

    private static void CreateWorld()
    {
      void placeTrees()
      {
        Model3d[] models = new Model3d[5];
        {
          int i = 0;
          foreach (string str in new[]
                     { "large_tree_1", "large_tree_2", "large_tree_3", "large_tree_4", "large_tree_5" })
          {
            Model3d model = Model3d.Read(str, new Dictionary<string, uint>());
            model.Scale(16f);
            models[i] = model;
            i++;
          }
        }

        for (int i = -100; i <= 100; i++)
        for (int j = -100; j <= 100; j++)
        {
          Entity obj = new()
          {
            Updates = true
          };
          Model3d.Component comp = new(models[Rand.Next(0, 5)], 0);
          obj.X = i * 50 + Rand.NextFloat(-12.5f, 12.5f);
          obj.Z = j * 50 + Rand.NextFloat(-12.5f, 12.5f);
          obj.Y = World.HeightAt((obj.X, obj.Z)) - 2f;
          obj.SetPrev();
          obj.Add(comp);
          obj.Add(new Tree());
          obj.Add(new MeshCollision<PC>(comp.Model.Mesh));
          World.Objs.Add(obj);
        }
      }

      void placeBushes()
      {
        Model3d[] models = new Model3d[3];
        {
          int i = 0;
          foreach (string str in new[] { "bush1", "bush2", "bush3" })
          {
            Model3d model = Model3d.Read(str, new Dictionary<string, uint>());
            model.Scale(16f);
            models[i] = model;
            i++;
          }
        }
        for (int i = -50; i <= 50; i++)
        for (int j = -50; j <= 50; j++)
        {
          if (Rand.Next(0, 3) != 0) continue;
          float ipos = i * 100 + Rand.NextFloat(-40, 40);
          float jpos = j * 100 + Rand.NextFloat(-40, 40);

          for (int k = 0; k < 3; k++)
          for (int l = 0; l < 3; l++)
          {
            if (Rand.Next(0, 3) != 0) continue;
            Entity obj = new();
            Model3d.Component comp = new(models[Rand.Next(0, 3)], 0);
            obj.Add(comp);
            obj.X = ipos + Rand.NextFloat(-24, 24);
            obj.Z = jpos + Rand.NextFloat(-24, 24);
            obj.Y = World.HeightAt((obj.X, obj.Z)) - 2f;
            obj.SetPrev();
            obj.Add(new MeshCollision<PC>(comp.Model.Mesh));
            World.Objs.Add(obj);
          }
        }
      }

      void makePlayer()
      {
        Player = new Entity
        {
          Updates = true
        };
        Player.Add(new Player());
        Player.Add(new Camera());
        Player.Yaw = Player.PrevYaw = 180;
        Player.X = Player.PrevX = Player.Z = Player.PrevZ = -1;
        Player.Y = Player.PrevY = 25;
        Player.Update();
      }

      makePlayer();
      World = new World();
      World.Objs.Add(Player);

      placeTrees();
      placeBushes();

      World.Update();
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      VSync = VSyncMode.Off;
      GL.DepthFunc(DepthFunction.Lequal);
      GlStateManager.EnableBlend();

      Ticker.Init();

      CursorState = CursorState.Grabbed;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

      if (e.Size == Vector2i.Zero)
        return;

      RenderSystem.UpdateProjection();
      RenderSystem.Resize();
      GL.Viewport(new Rectangle(0, 0, Size.X, Size.Y));
      Fbo.Resize(Size.X, Size.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);

      Camera.Get(Player).UpdateCameraVectors(Player);

      Fbo.Unbind();
      GL.ClearColor(_backgroundColor);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      RenderSystem.FRAME0.ClearColor();
      RenderSystem.FRAME0.ClearDepth();
      RenderSystem.FRAME0.Bind();
      GL.ClearColor(0f, 0f, 0f, 0f);

      RenderSystem.UpdateLookAt(Player);
      RenderSystem.UpdateProjection();

      World.Render();
      Model3d.Draw();

      RenderSystem.FRAME0.Bind();

      RenderSystem.FRAME0.Blit(RenderSystem.FRAME1.Handle);
      if (_outline)
      {
        RenderSystem.RenderOutline();
      }

      RenderSystem.RenderFxaa(RenderSystem.FRAME0);

      Fbo.Unbind();

      RenderSystem.FRAME0.Blit();

      RenderSystem.UpdateLookAt(Player, false);
      RenderSystem.UpdateProjection();

      if (FirstPerson)
      {
        RenderSystem.QUADS.Begin();
        RenderSystem.QUADS.Quad(
          RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(-5, -5, 0), PINK0)).Next(),
          RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(5, -5, 0), PINK0)).Next(),
          RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(5, 5, 0), PINK0)).Next(),
          RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(-5, 5, 0), PINK0)).Next()
        );
        if (HitResult.Type != HitResultType.None)
        {
          RenderSystem.QUADS.Quad(
            RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(-3, -3, 0), 0xffffffff)).Next(),
            RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(3, -3, 0), 0xffffffff)).Next(),
            RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(3, 3, 0), 0xffffffff)).Next(),
            RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f, Size.Y / 2f, 0) + new Vector3(-3, 3, 0), 0xffffffff)).Next()
          );
        }
        RenderSystem.QUADS.Render();
      }

      Font.Bind();
      RenderSystem.RenderingRed = true;
      RenderSystem.MESH.Begin();
      if (Frames % 3 == 0)
      {
        _mspf.Add(args.Time);
      }

      if (Environment.TickCount - _lastInquiry > 1000)
      {
        _lastInquiry = Environment.TickCount;
        _memUsage = (int)(GC.GetTotalMemory(false) / (1024 * 1024));
      }

      Font.Draw(RenderSystem.MESH, $"mspf: {_mspf.Average:N4} | fps: {1f / _mspf.Average:N2}", 11, 38, PINK0, false);
      Font.Draw(RenderSystem.MESH, $"time: {Environment.TickCount / 1000f % (MathF.PI * 2f):N2}", 11, 58, PINK0, false);
      Font.Draw(RenderSystem.MESH, $"xyz: {Player.Pos.X:N2}; {Player.Pos.Y:N2}; {Player.Pos.Z:N2}", 11, 78, PINK0, false);
      Font.Draw(RenderSystem.MESH, $"heap: {_memUsage}M", 11, 98, PINK0, false);
      Font.Draw(RenderSystem.MESH, $"rendered {InView} entities of {World.Objs.Count} ({Tris} tris)", 11, 118, PINK0, false);
      Font.Draw(RenderSystem.MESH, $"dist: {HitResult.Distance:N2}, type: {HitResult.Type}", 11, 138, PINK0, false);

      RenderSystem.MESH.Render();
      RenderSystem.RenderingRed = false;

      Font.Unbind();

      SwapBuffers();
      Frames++;
      Tris = 0;
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      MouseDx += e.DeltaX;
      MouseDy += e.DeltaY;
      MouseX = e.X;
      MouseY = e.Y;
    }

    public static bool IsPressed(Keys k)
    {
      return _keysDown.Contains(k);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);

      _lastTick = Now;

      if (KeyboardState.IsKeyDown(Keys.Escape))
      {
        Camera.Get(Player).FirstMouse = true;
        CursorState = CursorState.Normal;
      }

      foreach (Keys k in _keys)
      {
        if (k == Keys.Unknown) continue;
        if (KeyboardState.IsKeyPressed(k)) _keysDown.Add(k);
      }

      if (MouseState.WasButtonDown(MouseButton.Left)) CursorState = CursorState.Grabbed;

      if (KeyboardState.IsKeyPressed(Keys.O))
        _outline = !_outline;
      if (KeyboardState.IsKeyPressed(Keys.F11))
        WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
      if (KeyboardState.IsKeyPressed(Keys.F5))
        FirstPerson = !FirstPerson;
      
      Ticks++;

      World.Update();
      
      Vector3 eye = Player.LerpedPos + (0, 5, 0);
      Vector3 dir = Player.Get<Camera>(CompType.Camera).Front;
      HitResult = World.Raycast(eye, dir);

      MouseDx = MouseDy = 0;
      _keysDown.Clear();
    }
  }
}