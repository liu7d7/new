using New.Engine;

namespace New.Shared.Screens;

public sealed class ShaderSettingScreen : Screen
{
  public ShaderSettingScreen()
  {
    foreach (Shader s in Shader.Shaders)
    {
      Window w = new()
      {
        Title = s.Name
      };
      foreach (ShaderSetting setting in s.Settings)
      {
        w.AddChild(new ShaderSettingElement(setting));
      }
      AddChild(w);
    }
  }
}