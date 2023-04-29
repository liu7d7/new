using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace New.Shared.Screens;

public class Screen : Element
{
  public const int PADDING = 3;
  public const int IN_PADDING = 4;
  public const int TITLE_BAR_SIZE = 24;

  private static Vector2 GetMousePos()
  {
    Vector2d vec;
    unsafe
    {
      GLFW.GetCursorPos(Fall.Instance.WindowPtr, out vec.X, out vec.Y);
    }
    return (Vector2)vec;
  }
  public static int MouseX => (int)GetMousePos().X;
  public static int MouseY => (int)GetMousePos().Y;
  public static float MouseXf => GetMousePos().X;
  public static float MouseYf => GetMousePos().Y;
  public static int WinWidth => Fall.Instance.Size.X;
  public static int WinHeight => Fall.Instance.Size.Y;

  public override int X
  {
    get => 0;
    set => throw new NotImplementedException();
  }
  
  public override int Y
  {
    get => 0;
    set => throw new NotImplementedException();
  }
  
  public override int Width
  {
    get => WinWidth;
    set => throw new NotImplementedException();
  }
  
  public override int Height
  {
    get => WinHeight;
    set => throw new NotImplementedException();
  }

  public virtual void OnClose()
  {
    
  }
}