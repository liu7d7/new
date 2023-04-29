using New.Engine;
using OpenTK.Graphics.OpenGL4;

namespace New.Shared.Screens;

public class ShaderSettingElement : Element
{
  public override int X { get; set; }
  public override int Y { get; set; }
  public override int Width { get; set; }
  public override int Height { get; set; }
  
  private ShaderSetting _setting;
  
  public ShaderSettingElement(ShaderSetting setting)
  {
    _setting = setting;

    // switch (setting.Type)
    // {
    //   case ActiveUniformType.Float:
    //     AddChild(new SliderElement(setting));
    //     break;
    //   case ActiveUniformType.Int:
    //     AddChild(new SliderElement(setting));
    //     break;
    //   case ActiveUniformType.FloatVec2:
    //     AddChild(new SliderElement(setting));
    //     break;
    // }
  }
  
  public override void Render()
  {
    Element.DrawQuad(X, Y, Width, Height);
    Font.Draw(_setting.Name, X + Screen.PADDING, Y + Screen.PADDING, (uint)ElementColor.Pink, false);
    base.Render();
  }
}