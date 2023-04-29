using System.Drawing;
using System.Runtime.InteropServices;
using New.Engine;
using New.Shared;
using New.Shared.Components;
using New.Shared.Items;
using New.Shared.Screens;
using New.Shared.Worlds;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New;

public sealed class Fall : GameWindow
{
  public const uint PINK0 = 0xffff34b4;
  public static Fall Instance;
  public static Entity Player;
  public static World World;
  public static uint Ticks;
  public static uint Frames;
  public static int InView;
  public static int Tris;
  public static bool FirstPerson;
  public static HitResult Hit;
  public static Screen Screen;

  private static readonly DebugProc _debugProcCallback = DebugCallback;
  private static GCHandle _debugProcCallbackHandle;
  private static float _lastTick;
  private static float _tickLength = 50f;

  public static float Now;
  public static float TickDelta;
  private readonly Color4 _backgroundColor = Colors.NextColor();
  private readonly RollingAvg _mspf = new(120);
  private float _lastInquiry;
  private int _memUsage;
  private bool _outline = true;
  private bool _vsync;

  public Fall(GameWindowSettings windowSettings, NativeWindowSettings nativeWindowSettings) : base(windowSettings,
    nativeWindowSettings)
  {
    Instance = this;
    RenderSystem.Resize();
    CreateWorld();

    _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);
    GL.DebugMessageCallback(_debugProcCallback, nint.Zero);
    GL.Enable(EnableCap.DebugOutput);
    GL.Enable(EnableCap.DebugOutputSynchronous);
  }

  private static void DebugCallback(DebugSource source, DebugType type, int id,
    DebugSeverity severity, int length, nint message, nint userParam)
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
        foreach (string str in new[] { "large_tree_1", "large_tree_2", "large_tree_3", "large_tree_4", "large_tree_5" })
        {
          Model3d model = Model3d.Read(str);
          model.Scale(16f);
          models[i] = model;
          i++;
        }
      }

      for (int i = -100; i <= 100; i++)
      for (int j = -100; j <= 100; j++)
      {
        Entity obj = new()
          { Updates = true };
        obj.X = i * 50 + Rand.NextFloat(-12.5f, 12.5f);
        obj.Z = j * 50 + Rand.NextFloat(-12.5f, 12.5f);
        if (World.HeightAt(obj.X, obj.Z) < 0) continue;
        obj.Y = World.HeightAt((obj.X, obj.Z)) - 2f;
        obj.SetPrev();
        Model comp = new(models[Rand.Next(0, 5)], 0);
        obj.Add(comp);
        obj.Add(new Tree());
        obj.Add(new MeshCollision<PC>(comp.Model3d.Mesh));
        obj.Add(new Drop(Item.WOOD, 1, 3));
        obj.Add(new Life(10));
        World.Add(obj);
      }
    }

    void placeBushes()
    {
      Model3d[] models = new Model3d[3];
      {
        int i = 0;
        foreach (string str in new[] { "bush1", "bush2", "bush3" })
        {
          Model3d model = Model3d.Read(str);
          model.Scale(16f);
          models[i] = model;
          i++;
        }
      }
      for (int i = -50; i <= 50; i++)
      for (int j = -50; j <= 50; j++)
      {
        if (Rand.Next(0, 3) == 2) continue;
        float ipos = i * 100 + Rand.NextFloat(-40, 40);
        float jpos = j * 100 + Rand.NextFloat(-40, 40);

        for (int k = 0; k < 3; k++)
        for (int l = 0; l < 3; l++)
        {
          if (Rand.Next(0, 3) != 0) continue;
          Entity obj = new();
          obj.X = ipos + Rand.NextFloat(-24, 24);
          obj.Z = jpos + Rand.NextFloat(-24, 24);
          if (World.HeightAt(obj.X, obj.Z) < 0) continue;
          obj.Y = World.HeightAt((obj.X, obj.Z)) - 2f;
          obj.SetPrev();
          Model comp = new(models[Rand.Next(0, 3)], 0);
          obj.Add(comp);
          obj.Add(new MeshCollision<PC>(comp.Model3d.Mesh));
          obj.Add(new Life(5));
          World.Add(obj);
        }
      }
    }

    void makePlayer()
    {
      Player = new Entity { Updates = true };
      Player.Add(new Play());
      Player.Add(new Camera());
      Player.Add(new Inventory());
      Player.Yaw = Player.PrevYaw = 180;
      Player.X = Player.PrevX = Player.Z = Player.PrevZ = -1;
      Player.Y = Player.PrevY = 25;
      Camera c = Player.Get<Camera>(CompType.Camera);
      c.UpdateVectors(Player);
      Player.Update();
    }

    makePlayer();
    World = new World();
    World.Add(Player);
    placeTrees();
    placeBushes();
    World.Update();
  }

  protected override void OnRenderFrame(FrameEventArgs args)
  {
    base.OnRenderFrame(args);

    Now = (float)GLFW.GetTime() * 1000f;
    TickDelta = (Now - _lastTick) / _tickLength;

    Camera c = Player.Get<Camera>(CompType.Camera);
    c.UpdateVectors(Player);

    Fbo.Unbind();
    GL.ClearColor(_backgroundColor);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    RenderSystem.FRAME0.ClearColor();
    RenderSystem.FRAME0.ClearDepth();
    RenderSystem.FRAME0.Bind();
    GL.ClearColor(0f, 0f, 0f, 0f);

    RenderSystem.Look(Player);
    RenderSystem.Project();

    World.Render();
    Model3d.Draw();
    
    RenderSystem.Outline();
    
    RenderSystem.FRAME1.GLBlit(RenderSystem.FRAME0.Handle);
    RenderSystem.Fxaa(RenderSystem.FRAME0);
    
    Fbo.Unbind();

    RenderSystem.FRAME0.Blit();

    RenderSystem.Look(Player, false);
    RenderSystem.Project();
    
    RenderSystem.QUADS.Begin();

    if (FirstPerson)
    {
      RenderSystem.QUADS.Quad(
        RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f - 5, Size.Y / 2f - 5, 0), PINK0)),
        RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f + 5, Size.Y / 2f - 5, 0), PINK0)),
        RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f + 5, Size.Y / 2f + 5, 0), PINK0)),
        RenderSystem.QUADS.Put(new(new Vector3(Size.X / 2f - 5, Size.Y / 2f + 5, 0), PINK0))
      );
      if (Hit.Type != HitResultType.None)
        RenderSystem.QUADS.Quad(
          RenderSystem.QUADS.Put(new(Size.X / 2f - 3, Size.Y / 2f - 3, 0, 0xffffffff)),
          RenderSystem.QUADS.Put(new(Size.X / 2f + 3, Size.Y / 2f - 3, 0, 0xffffffff)),
          RenderSystem.QUADS.Put(new(Size.X / 2f + 3, Size.Y / 2f + 3, 0, 0xffffffff)),
          RenderSystem.QUADS.Put(new(Size.X / 2f - 3, Size.Y / 2f + 3, 0, 0xffffffff))
        );
    }
    
    RenderSystem.QUADS.Render();

    Font.Bind();
    RenderSystem.FONT.Begin();

    if (Frames % 3 == 0) _mspf.Add(args.Time);

    if (Now - _lastInquiry > 1000)
    {
      _lastInquiry = Now;
      _memUsage = (int)(GC.GetTotalMemory(false) / 1024);
    }

    Font.Draw($"mspf: {_mspf.Average * 1000:N4} | fps: {1f / _mspf.Average:N2}", 11, 11, PINK0, false);
    Font.Draw($"time: {Now / 1000f % (MathF.PI * 2f):N2}", 11, 31, PINK0, false);
    Font.Draw($"xyz: {Player.X:N2}; {Player.Y:N2}; {Player.Z:N2}", 11, 51, PINK0, false);
    Font.Draw($"heap: {_memUsage}K", 11, 71, PINK0, false);
    Font.Draw($"rendered {InView} entities of {World.Objs.Count} ({Tris} tris)", 11, 91, PINK0, false);
    Font.Draw($"dist: {Hit.Distance:N2}, type: {Hit.Type}", 11, 111, PINK0, false);
    Font.Draw($"pool efficiency: {(float)World.Objs.Count / ComponentPool.TheoreticalCount:N6}", 11, 131, PINK0, false);
    
    RenderSystem.FONT.Render();

    RenderSystem.QUADS.Begin();
    RenderSystem.LINE.Begin();
    RenderSystem.FONT.Begin();

    Screen?.Render();
    
    RenderSystem.QUADS.Render();
    RenderSystem.LINE.Render();
    RenderSystem.FONT.Render();

    Font.Unbind();

    SwapBuffers();
    Frames++;
    Tris = 0;
  }

  protected override void OnUpdateFrame(FrameEventArgs args)
  {
    base.OnUpdateFrame(args);

    _tickLength = (float)(args.Time * 1000f);
    _lastTick = Now;

    if (MouseState.IsButtonPressed(MouseButton.Left) && Screen == null)
      CursorState = CursorState.Grabbed;

    if (KeyboardState.IsKeyPressed(Keys.O))
    {
      Screen = new Screen();
      Camera.Get(Player).FirstMouse = true;
      CursorState = CursorState.Normal;
    }
    
    if (KeyboardState.IsKeyPressed(Keys.Escape))
    {
      if (Screen != null)
      {
        Screen.OnClose();
        Screen = null;
        CursorState = CursorState.Grabbed;
      }
      else
      {
        Camera.Get(Player).FirstMouse = true;
        CursorState = CursorState.Normal;
      }
    }
    if (KeyboardState.IsKeyPressed(Keys.F11))
      WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
    if (KeyboardState.IsKeyPressed(Keys.V))
      FirstPerson = !FirstPerson;
    
    Ticks++;
    
    HandleInput();

    Screen?.Update();
    World.Update();

    Vector3 eye = (Player.LerpedX, Player.LerpedY + 5, Player.LerpedZ);
    Vector3 dir = Player.Get<Camera>(CompType.Camera).Front;
    Hit = World.Raycast(eye, dir);
  }
  
  private void HandleInput()
  {
    if (Screen != null) return;
    
    Play play = Play.Get(Player);
    
    if (MouseState.IsButtonDown(MouseButton.Left) && play.CanHit())
    {
      play.LeftProgress = Now;
      Hit.Entity?.Interact(new Interaction(Player, Hand.Left, InteractType.Hit));
    }
    else if (MouseState.IsButtonDown(MouseButton.Right) && play.CanHit())
    {
      play.RightProgress = Now;
      Hit.Entity?.Interact(new Interaction(Player, Hand.Right, InteractType.Hit));
    }
    else if (KeyboardState.IsKeyPressed(Keys.F))
    {
      Hit.Entity?.Interact(new Interaction(Player, Hand.Left, InteractType.Pickup));
    }
  }

  protected override void OnMouseMove(MouseMoveEventArgs e)
  {
    base.OnMouseMove(e);
    
    Screen?.MouseMove(e);
  }

  protected override void OnKeyDown(KeyboardKeyEventArgs e)
  {
    base.OnKeyDown(e);

    Screen?.KeyDown(e);
  }
  
  protected override void OnKeyUp(KeyboardKeyEventArgs e)
  {
    base.OnKeyUp(e);

    Screen?.KeyUp(e);
  }
  
  protected override void OnMouseDown(MouseButtonEventArgs e)
  {
    base.OnMouseDown(e);

    Screen?.MouseDown(e);
  }
  
  protected override void OnMouseUp(MouseButtonEventArgs e)
  {
    base.OnMouseUp(e);

    Screen?.MouseUp(e);
  }

  protected override void OnLoad()
  {
    base.OnLoad();

    VSync = VSyncMode.Off;
    GL.DepthFunc(DepthFunction.Lequal);
    GlStateManager.EnableBlend();

    CursorState = CursorState.Grabbed;
  }

  protected override void OnResize(ResizeEventArgs e)
  {
    base.OnResize(e);

    if (e.Size == Vector2i.Zero)
      return;

    RenderSystem.Project();
    RenderSystem.Resize();
    GL.Viewport(new Rectangle(0, 0, Size.X, Size.Y));
    Fbo.Resize(Size.X, Size.Y);
  }
}