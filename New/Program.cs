using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New;

public static class Program
{
  // ReSharper disable once InconsistentNaming
  [STAThread]
  public static void Main(string[] args)
  {
    NativeWindowSettings nativeWindowSettings = new()
    {
      Size = new Vector2i(1152, 720),
      Title = "Fall",
      Flags = ContextFlags.ForwardCompatible
    };

    GameWindowSettings gameWindowSettings = new()
    {
      RenderFrequency = 0,
      UpdateFrequency = 20
    };

    GLFW.Init();
    GLFW.WindowHint(WindowHintInt.Samples, 4);
    using Fall fall = new(gameWindowSettings, nativeWindowSettings);
    fall.Run();
  }
}