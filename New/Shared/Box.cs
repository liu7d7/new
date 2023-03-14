using OpenTK.Mathematics;

namespace New.Shared
{
  public struct Box
  {
    public Vector3 Min = new Vector3();
    public Vector3 Max = new Vector3();

    public Box(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
    {
      Min.X = minX;
      Min.Y = minY;
      Min.Z = minZ;
      Max.X = maxX;
      Max.Y = maxY;
      Max.Z = maxZ;
    }

    public Box(Vector3 min, Vector3 max)
    {
      Min = min;
      Max = max;
    }

    public bool RayCollides(Vector3 offset, Vector3 ro, Vector3 rd, out float dist)
    {
      Vector3 dirfrac = (1.0f / rd.X, 1.0f / rd.Y, 1.0f / rd.Z);
      Vector3 lb = Min + offset;
      Vector3 rt = Max + offset;

      float t1 = (lb.X - ro.X) * dirfrac.X;
      float t2 = (rt.X - ro.X) * dirfrac.X;
      float t3 = (lb.Y - ro.Y) * dirfrac.Y;
      float t4 = (rt.Y - ro.Y) * dirfrac.Y;
      float t5 = (lb.Z - ro.Z) * dirfrac.Z;
      float t6 = (rt.Z - ro.Z) * dirfrac.Z;

      float tmin = MathF.Max(MathF.Max(MathF.Min(t1, t2), MathF.Min(t3, t4)), MathF.Min(t5, t6));
      float tmax = MathF.Min(MathF.Min(MathF.Max(t1, t2), MathF.Max(t3, t4)), MathF.Max(t5, t6));

      if (tmax < 0)
      {
        dist = tmax;
        return false;
      }

      if (tmin > tmax)
      {
        dist = tmax;
        return false;
      }

      dist = tmin;
      return true;
    }
  }
}