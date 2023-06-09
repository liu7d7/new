using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using New.Engine;
using OpenTK.Mathematics;

namespace New.Shared;

public static class DeterministicRandom
{
  private static readonly float[] _rand = new float[1000];

  static DeterministicRandom()
  {
    for (int i = 0; i < 1000; i++) _rand[i] = Rand.NextFloat();
  }

  public static float next_float(object val)
  {
    return _rand[Math.Abs(val.GetHashCode()) % 1000];
  }

  public static int NextInt(object val, int max)
  {
    return (int)(_rand[Math.Abs(val.GetHashCode()) % 1000] * max);
  }
}

public static class Colors
{
  private static bool _initialized;
  private static readonly Dictionary<string, Color4> _values = new();
  private static readonly Vec<Color4> _colors = new();

  private static float _red, _blue = 0.75f, _green = 1.5f;

  public static Color4 GetColor(string color)
  {
    if (_initialized) return _values[color.ToLower()];
    foreach (PropertyInfo john in typeof(Color4).GetProperties())
    {
      object val = john.GetValue(null);

      if (val is not Color4 color4) continue;
      _values.Add(john.Name.ToLower(), color4);
      _colors.Add(color4);
    }

    _initialized = true;
    return _values[color.ToLower()];
  }

  public static Color4 GetRandomColor(object val = null)
  {
    val ??= Rand.NextLong();
    if (!_initialized) GetColor("white");
    return _colors[DeterministicRandom.NextInt(val, _colors.Count)];
  }

  public static Color4 NextColor()
  {
    _red += RenderSystem.THRESHOLD;
    _blue += RenderSystem.THRESHOLD * 2;
    _green += RenderSystem.THRESHOLD * 3;
    _red %= 1f;
    _blue %= 1f;
    _green %= 1f;
    return new Color4(_red, _green, _blue, 1f);
  }

  public static uint ToUint(this Color4 color)
  {
    return (uint)(color.A * 255) << 24 | (uint)(color.R * 255) << 16 | (uint)(color.G * 255) << 8 |
           (uint)(color.B * 255);
  }
}

public static class Rand
{
  public static float NextFloat()
  {
    return Random.Shared.NextSingle();
  }

  public static float NextFloat(float min, float max)
  {
    return Random.Shared.NextSingle() * (max - min) + min;
  }

  public static int Next(int min, int max)
  {
    return Random.Shared.Next(min, max);
  }

  public static int Next(int max)
  {
    return Random.Shared.Next(max);
  }

  public static int Next()
  {
    return Random.Shared.Next();
  }
  
  public static uint NextUInt(uint min, uint max)
  {
    return (uint)Next((int)min, (int)max);
  }

  public static long NextLong()
  {
    return Random.Shared.NextInt64();
  }
}

public static class Extensions
{
  public static string ContentToString<T>(this IEnumerable<T> arr)
  {
    string o = "[";
    foreach (T item in arr) o += item + ", ";
    if (o.Length != 1)
      o = o[..^2];
    o += "]";
    return o;
  }

  public static string ContentToString<T, Tv>(this Dictionary<T, Tv> arr)
  {
    StringBuilder o = new("Map<");
    o.Append(typeof(T));
    o.Append(", ");
    o.Append(typeof(Tv));
    o.Append(">(");
    foreach (KeyValuePair<T, Tv> item in arr)
    {
      o.Append('(');
      o.Append(item.Key);
      o.Append(", ");
      o.Append(item.Value);
      o.Append("), ");
    }

    if (arr.Count > 0) o.Remove(o.Length - 3, 3);
    o.Append(')');

    return o.ToString();
  }

  public static Task for_each_async<T>(Vec<T> vec, int batches, Action<T> action)
  {
    async Task partition(IEnumerator<T> part)
    {
      using (part)
      {
        while (part.MoveNext()) await Task.Run(() => action(part.Current));
      }
    }

    return Task.WhenAll(Partitioner.Create(vec).GetPartitions(batches).AsParallel().Select(partition));
  }

  public static Task for_async(Range list, int batches, Action<int> action)
  {
    async Task partition(IEnumerator<int> part)
    {
      using (part)
      {
        while (part.MoveNext()) await Task.Run(() => action(part.Current));
      }
    }

    return Task.WhenAll(Partitioner.Create(Enumerable.Range(list.Start.Value, list.End.Value)).GetPartitions(batches)
      .AsParallel().Select(partition));
  }

  private const float _toRad = 1f / 180f * MathF.PI;
  private const float _toDeg = 1f / MathF.PI * 180f;

  public static float Rad(this float degrees)
  {
    return degrees * _toRad;
  }

  public static float Deg(this float radians)
  {
    return radians * _toDeg;
  }

  public static void Scale(this ref Matrix4 matrix4, float scalar)
  {
    matrix4 *= Matrix4.CreateScale(scalar);
  }

  public static void Translate(this ref Matrix4 matrix4, Vector3 translation)
  {
    matrix4 *= Matrix4.CreateTranslation(translation);
  }

  public static void Translate(this ref Matrix4 matrix4, float x, float y, float z)
  {
    matrix4 *= Matrix4.CreateTranslation(x, y, z);
  }

  public static void Rotate(this ref Matrix4 matrix4, float angle, float x, float y, float z)
  {
    matrix4 *= Matrix4.CreateFromAxisAngle(new Vector3(x, y, z), angle / 180f * MathF.PI);
  }

  public static Vector2i to_chunk_pos(this Vector2 vec)
  {
    return ((int)vec.X >> 4, (int)vec.Y >> 4);
  }

  public static Vector2i to_chunk_pos(this Vector3 vec)
  {
    return ((int)vec.X >> 4, (int)vec.Z >> 4);
  }
}