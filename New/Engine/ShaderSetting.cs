using OpenTK.Graphics.OpenGL4;
 using OpenTK.Mathematics;

 namespace New.Engine;

public class ShaderSetting
{
  public string Name;
  public ActiveUniformType Type;
  public float Min;
  public float Max;

  public static readonly HashSet<ActiveUniformType> TYPES = new()
  {
    ActiveUniformType.Float,
    ActiveUniformType.Int,
    ActiveUniformType.FloatVec2,
    ActiveUniformType.FloatVec3,
    ActiveUniformType.FloatVec4
  };

  protected ShaderSetting(string config, string line, ActiveUniformType type)
  {
    config = config.Substring(2);
    if (config.StartsWith(' ')) config = config[1..];
    if (config.EndsWith("*/")) config = config[..^2];
    if (config.EndsWith(' ')) config = config[..^1];
    string[] split = config.Split("..");
    Min = float.Parse(split[0]);
    Max = float.Parse(split[1]);
    Name = line.Split(' ')[2];
    Type = type;
  }

  public static ShaderSetting Read(string config, string line)
  {
    string[] split = line.Split(' ');
    string name = split[2];
    ActiveUniformType type = GetType(line);
    
    return type switch
    {
      ActiveUniformType.Float => new FloatShaderSetting(config, line),
      ActiveUniformType.Int => new IntShaderSetting(config, line),
      ActiveUniformType.FloatVec2 => new Vec2ShaderSetting(config, line),
      ActiveUniformType.FloatVec3 => new Vec3ShaderSetting(config, line),
      ActiveUniformType.FloatVec4 => new Vec4ShaderSetting(config, line),
      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

  public static string GetTypeName(ActiveUniformType type)
  {
    return type switch
    {
      ActiveUniformType.Float => "float",
      ActiveUniformType.Int => "int",
      ActiveUniformType.FloatVec2 => "vec2",
      ActiveUniformType.FloatVec3 => "vec3",
      ActiveUniformType.FloatVec4 => "vec4",
      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

  public static float ParseFloat(string s)
  {
    if (s.EndsWith('.')) s += '0';
    return float.Parse(s);
  }

  public static Vector2 ParseVec2(string s)
  {
    // vec2(0., 0.) -> {0., 0.}
    string[] split = s[5..^1].Split(", ");

    Vector2 ret = split.Length switch
    {
      1 => new Vector2(ParseFloat(split[0]), ParseFloat(split[0])),
      2 => new Vector2(ParseFloat(split[0]), ParseFloat(split[1])),
      _ => throw new ArgumentException($"{s} did not contain 1-2 floats", nameof(s))
    };
    
    Console.WriteLine($"{s} => {ret}");
    return ret;
  }

  public static Vector3 ParseVec3(string s)
  {
    // vec3(0., 0., 0.) -> {0., 0., 0.}
    string[] split = s[5..^1].Split(", ");

    Vector3 ret = split.Length switch
    {
      1 => new Vector3(ParseFloat(split[0]), ParseFloat(split[0]), ParseFloat(split[0])),
      3 => new Vector3(ParseFloat(split[0]), ParseFloat(split[1]), ParseFloat(split[2])),
      _ => throw new ArgumentException($"{s} did not contain 1-3 floats", nameof(s))
    };
    
    Console.WriteLine($"{s} => {ret}");
    return ret;
  }
  
  public static Vector4 ParseVec4(string s)
  {
    // vec4(0., 0., 0., 0.) -> {0., 0., 0., 0.}
    string[] split = s[5..^1].Split(", ");

    Vector4 ret = split.Length switch
    {
      1 => new Vector4(ParseFloat(split[0]), ParseFloat(split[0]), ParseFloat(split[0]), ParseFloat(split[0])),
      4 => new Vector4(ParseFloat(split[0]), ParseFloat(split[1]), ParseFloat(split[2]), ParseFloat(split[3])),
      _ => throw new ArgumentException($"{s} did not contain 1-4 floats", nameof(s))
    };
    
    Console.WriteLine($"{s} => {ret}");
    return ret;
  }
  
  public static ActiveUniformType GetType(string s)
  {
    string[] split = s.Split(" ");
    if (split[0] != "uniform") throw new ArgumentException($"{s} is not a uniform", nameof(s));
    return split[1] switch
    {
      "float" => ActiveUniformType.Float,
      "int" => ActiveUniformType.Int,
      "vec2" => ActiveUniformType.FloatVec2,
      "vec3" => ActiveUniformType.FloatVec3,
      "vec4" => ActiveUniformType.FloatVec4,
      _ => throw new ArgumentException($"{s} is not a valid type", nameof(s))
    };
  }
  
}

public sealed class FloatShaderSetting : ShaderSetting
{
 public float Value;

 public FloatShaderSetting(string config, string line) : base(config, line, ActiveUniformType.Float)
 {
   Value = ShaderSetting.ParseFloat(string.Join(' ', line.Split(' ')[4..])[..^1]);
 }
}
 
public sealed class IntShaderSetting : ShaderSetting
{
  public int Value;

  public IntShaderSetting(string config, string line) : base(config, line, ActiveUniformType.Int)
  {
    Value = int.Parse(string.Join(' ', line.Split(' ')[4..])[..^1]);
  }
}

public sealed class Vec2ShaderSetting : ShaderSetting
{
  public Vector2 Value;

  public Vec2ShaderSetting(string config, string line) : base(config, line, ActiveUniformType.FloatVec2)
  {
    Value = ShaderSetting.ParseVec2(string.Join(' ', line.Split(' ')[4..])[..^1]);
  }
}

public sealed class Vec3ShaderSetting : ShaderSetting
{
  public Vector3 Value;

  public Vec3ShaderSetting(string config, string line) : base(config, line, ActiveUniformType.FloatVec3)
  {
    Value = ShaderSetting.ParseVec3(string.Join(' ', line.Split(' ')[4..])[..^1]);
  }
}

public sealed class Vec4ShaderSetting : ShaderSetting
{
  public Vector4 Value;

  public Vec4ShaderSetting(string config, string line) : base(config, line, ActiveUniformType.FloatVec4)
  {
    Value = ShaderSetting.ParseVec4(string.Join(' ', line.Split(' ')[4..])[..^1]);
  }
}