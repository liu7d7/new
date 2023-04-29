using New.Engine;
using OpenTK.Windowing.Common;

namespace New.Shared.Screens;

public enum ElementColor : uint
{
  Pink = Fall.PINK0,
  White = 0xffffffff
}

public abstract class Element
{
  public abstract int X { get; set; }
  public abstract int Y { get; set; }
  public abstract int Width { get; set; }
  public abstract int Height { get; set; }
  public bool Removed { get; set; }
  public Vec<Element> Children { get; set; } = new();
  
  public virtual bool InBounds(int x, int y) => x >= X && x < X + Width && y >= Y && y < Y + Height;
  
  public virtual void Render()
  {
    Children.RemoveAll(e => e.Removed);
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
      children[i].Render();
  }

  public virtual void Update()
  {
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
      children[i].Update();
  }

  public virtual void MouseMove(MouseMoveEventArgs ev)
  {
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
      children[i].MouseMove(ev);
  }

  public virtual bool MouseDown(MouseButtonEventArgs ev)
  {
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
    {
      if (!children[i].InBounds(Screen.MouseX, Screen.MouseY)) continue;
      if (children[i].MouseDown(ev))
      {
        return true;
      }
    }

    return false;
  }

  public virtual bool MouseUp(MouseButtonEventArgs ev)
  {
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
    {
      if (!children[i].InBounds(Screen.MouseX, Screen.MouseY)) continue;
      if (children[i].MouseUp(ev))
      {
        return true;
      }
    }

    return false;
  }
  
  public virtual bool KeyDown(KeyboardKeyEventArgs ev)
  {
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
    {
      if (children[i].KeyDown(ev))
      {
        return true;
      }
    }

    return false;
  }
  
  public virtual bool KeyUp(KeyboardKeyEventArgs ev)
  {
    Span<Element> children = Children.Items;
    for (int i = 0; i < Children.Count; i++)
    {
      if (children[i].KeyUp(ev))
      {
        return true;
      }
    }

    return false;
  }

  public void AddChild(Element element)
  {
    Children.Add(element);
    OnAddChild();
  }

  public virtual void OnAddChild()
  {
    Span<Element> children = Children.Items;
    int y = Y + Screen.PADDING, maxWidth = Width, totalHeight = 0;
    for (int i = 0; i < children.Length; i++)
    {
      Element child = children[i];
      child.X = X + Screen.PADDING;
      child.Y = y;
      y += child.Height + Screen.PADDING;
      maxWidth = Math.Max(maxWidth, child.Width);
      totalHeight += child.Height + Screen.PADDING;
    }

    Width = maxWidth;
    Height = totalHeight;
  }

  public static void DrawFilledQuad(int x, int y, int w, int h, ElementColor color)
  {
    Mesh<PC> mesh = RenderSystem.QUADS;
    mesh.Quad(
      mesh.Put(new(x, y, 0, (uint)color)),
      mesh.Put(new(x + w, y, 0, (uint)color)),
      mesh.Put(new(x + w, y + h, 0, (uint)color)),
      mesh.Put(new(x, y + h, 0, (uint)color))
    );
  }

  public static void DrawQuad(int x, int y, int w, int h)
  {
    DrawFilledQuad(x, y, w, h, ElementColor.White);
    DrawFilledQuad(x, y, w, 2, ElementColor.Pink);
    DrawFilledQuad(x, y + h - 2, w, 2, ElementColor.Pink);
    DrawFilledQuad(x, y, 2, h, ElementColor.Pink);
    DrawFilledQuad(x + w - 2, y, 2, h, ElementColor.Pink);
  }
}