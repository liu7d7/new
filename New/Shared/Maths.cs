using OpenTK.Mathematics;

namespace New.Shared;

public static class Maths
{
  public static float calc_angle(float v, float h)
  {
    return MathF.Atan2(v, h).Deg();
  }

  public static float calc_angle_xz(Entity eye, Entity target)
  {
    return calc_angle(target.Z - eye.Z, target.X - eye.X);
  }

  public static float wrap_degrees(float degrees)
  {
    float f = degrees % 360f;
    return f switch
    {
      >= 180f => f - 360f,
      < -180f => f + 360f,
      _ => f
    };
  }

  public static float Lerp(float start, float end, float delta)
  {
    return start + (end - start) * delta;
  }

  public static void Clamp(ref int val, int start, int end)
  {
    val = Math.Min(Math.Max(val, start), end);
  }

  public static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

  public static void Cross(in Vector3 left, in Vector3 right, out Vector3 result)
  {
    result.X = left.Y * right.Z - left.Z * right.Y;
    result.Y = left.Z * right.X - left.X * right.Z;
    result.Z = left.X * right.Y - left.Y * right.X;
  }

  public static Vector3 Cross(Vector3 left, Vector3 right)
  {
    Vector3 result;
    Cross(in left, in right, out result);
    return result;
  }
}