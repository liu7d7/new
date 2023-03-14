using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public class FloatPos : Entity.Component
  {
    public float Pitch;
    public float PrevPitch;
    public float PrevX;
    public float PrevY;
    public float PrevYaw;
    public float PrevZ;
    public float X;
    public float Y;
    public float Yaw;
    public float Z;

    public FloatPos() : base(CompType.FloatPos)
    {
      X = PrevX = Y = PrevY = Z = PrevZ = Yaw = PrevYaw = Pitch = PrevPitch = 0;
    }

    public float LerpedX => Math.Lerp(PrevX, X, Ticker.TickDelta);
    public float LerpedY => Math.Lerp(PrevY, Y, Ticker.TickDelta);
    public float LerpedZ => Math.Lerp(PrevZ, Z, Ticker.TickDelta);
    public float LerpedYaw => Math.Lerp(PrevYaw, Yaw, Ticker.TickDelta);
    public float LerpedPitch => Math.Lerp(PrevPitch, Pitch, Ticker.TickDelta);

    public static FloatPos Get(Entity obj)
    {
      return obj.Get<FloatPos>(CompType.FloatPos);
    }

    public Vector3 ToVec3()
    {
      return (X, Y, Z);
    }

    public void SetVec3(Vector3 pos)
    {
      (X, Y, Z) = pos;
    }

    public void IncVec3(Vector3 inc)
    {
      X += inc.X;
      Y += inc.Y;
      Z += inc.Z;
    }

    public Vector3 ToLerpedVec3(float xOff = 0, float yOff = 0, float zOff = 0)
    {
      return (LerpedX + xOff, LerpedY + yOff, LerpedZ + zOff);
    }

    public void SetPrev()
    {
      PrevX = X;
      PrevY = Y;
      PrevZ = Z;
      PrevYaw = Yaw;
      PrevPitch = Pitch;
    }
  }
}