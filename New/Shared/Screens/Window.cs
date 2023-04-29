using New.Engine;
using OpenTK.Windowing.Common;

namespace New.Shared.Screens;

public class Window : Element
{
  public override int X { get; set; }
  public override int Y { get; set; }
  public override int Width { get; set; }
  public override int Height { get; set; }
  public string Title { get; init; }

  private bool _dragging;
  private int _dragX, _dragY;

  public Window()
  {
    AddChild(new WindowButton(this, () => Removed = true, e =>
    {
      RenderSystem.Line(e.X + Screen.IN_PADDING, e.Y + Screen.IN_PADDING, e.X + e.Width - Screen.IN_PADDING, e.Y + e.Height - Screen.IN_PADDING, 1, (uint)ElementColor.Pink);
      RenderSystem.Line(e.X + Screen.IN_PADDING, e.Y + e.Height - Screen.IN_PADDING, e.X + e.Width - Screen.IN_PADDING, e.Y + Screen.IN_PADDING, 1, (uint)ElementColor.Pink);
    }, 1));
  }
  
  public bool InTitleBar(int x, int y) => x >= X && x < X + Width && y >= Y - Screen.TITLE_BAR_SIZE && y < Y;

  public override bool InBounds(int x, int y) => base.InBounds(x, y) || InTitleBar(x, y);

  private void RenderBackground()
  {
    Element.DrawQuad(X, Y - Screen.TITLE_BAR_SIZE, Width, Screen.TITLE_BAR_SIZE);
    Font.Draw(Title, X + 2, Y - Screen.TITLE_BAR_SIZE + Screen.PADDING, (uint)ElementColor.Pink, false);
    Element.DrawQuad(X, Y - 2, Width, Height);
  }

  public override void Render()
  {
    if (_dragging)
    {
      X = Screen.MouseX - _dragX;
      Y = Screen.MouseY - Screen.TITLE_BAR_SIZE - _dragY;
    }
    
    RenderBackground();
    base.Render();
  }

  public override bool MouseDown(MouseButtonEventArgs ev)
  {
    if (base.MouseDown(ev)) return true;

    if (!InTitleBar(Screen.MouseX, Screen.MouseY)) return false;
    
    _dragging = true;
    _dragX = Screen.MouseX - X;
    _dragY = Screen.MouseY - Y - Screen.TITLE_BAR_SIZE;
    return true;
  }

  public override bool MouseUp(MouseButtonEventArgs ev)
  {
    base.MouseUp(ev);
    
    _dragging = false;
    return false;
  }

  private class WindowButton : Element
  {
    public override int X
    {
      get => _window.X + _window.Width - (_width + Screen.PADDING) * _offX;
      set => throw new NotImplementedException();
    }

    public override int Y
    {
      get => _window.Y - Screen.TITLE_BAR_SIZE + Screen.PADDING; 
      set => throw new NotImplementedException();
    }

    public override int Width
    {
      get => _width;
      set => throw new NotImplementedException();
    }

    public override int Height
    {
      get => _width;
      set => throw new NotImplementedException();
    }
    
    private Window _window;
    private int _offX;
    private Action _onClick;
    private Action<Element> _onRender;

    private const int _width = Screen.TITLE_BAR_SIZE - Screen.PADDING * 2;
    
    public WindowButton(Window window, Action onClick, Action<Element> onRender, int offX)
    {
      _window = window;
      _offX = offX;
      _onClick = onClick;
      _onRender = onRender;
    }

    public override void Render()
    {
      Element.DrawQuad(X, Y, Width, Height);
      _onRender(this);
    }

    public override bool MouseDown(MouseButtonEventArgs ev)
    {
      if (base.MouseDown(ev)) return true;
      
      _onClick();
      return true;
    }
  }
}