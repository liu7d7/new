using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public class FloatPosStatic : Component
  {
    public float X;
    public float Y;
    public float Z;

    public FloatPosStatic() : base(CompType.FloatPosStatic)
    {
    }

    public static FloatPosStatic Get(Entity obj)
    {
      return obj.Get<FloatPosStatic>(CompType.FloatPosStatic);
    }

    public Vector3 ToVec3()
    {
      return (X, Y, Z);
    }
  }
}