using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public class FloatPosStatic : Entity.Component
  {
    public float X;
    public float Y;
    public float Z;

    public FloatPosStatic() : base(Entity.CompType.FloatPosStatic)
    {
    }

    public static FloatPosStatic Get(Entity obj)
    {
      return obj.Get<FloatPosStatic>(Entity.CompType.FloatPosStatic);
    }

    public Vector3 ToVec3()
    {
      return (X, Y, Z);
    }
  }
}