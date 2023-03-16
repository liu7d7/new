namespace New.Shared
{
  public static class Math
  {
    public static float CalcAngle(float v, float h)
    {
      return MathF.Atan2(v, h).Deg();
    }

    public static float CalcAngleXz(Entity eye, Entity target)
    {
      return CalcAngle(target.Z - eye.Z, target.X - eye.X);
    }

    public static float WrapDegrees(float degrees)
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
      val = System.Math.Min(System.Math.Max(val, start), end);
    }
  }
}