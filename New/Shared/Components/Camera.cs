using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New.Shared.Components
{
  public class Camera : Component
  {
    public const float FOV = 45 * MathF.PI / 180f;
    public const float NEAR = 0.1f;
    private readonly Vector3 _up;
    private float _lastX;
    private float _lastY;
    private FloatPos _pos;
    private Vector3 _right;

    private Vector3 _velocity;

    public bool FirstMouse = true;
    public Vector3 Front;

    public Camera() : base(CompType.Camera)
    {
      Front = Vector3.Zero;
      _right = Vector3.Zero;
      _up = Vector3.UnitY;
      _lastX = 0;
    }

    public static float Far => Fall.FarCamera ? 1024f : 128f;

    public static Camera Get(Entity obj)
    {
      return obj.Get<Camera>(CompType.Camera);
    }

    public void UpdateCameraVectors()
    {
      Front = new Vector3(MathF.Cos(_pos.LerpedPitch.Rad()) * MathF.Cos(_pos.LerpedYaw.Rad()),
        MathF.Sin(_pos.LerpedPitch.Rad()),
        MathF.Cos(_pos.LerpedPitch.Rad()) * MathF.Sin(_pos.LerpedYaw.Rad())).Normalized();
      _right = Vector3.Cross(Front, _up).Normalized();
    }

    public override void Update(Entity objIn)
    {
      base.Update(objIn);

      _pos ??= FloatPos.Get(objIn);

      _pos.SetPrev();

      OnMouseMove();

      int forwards = 0;
      int rightwards = 0;
      KeyboardState kb = Fall.Instance.KeyboardState;
      if (kb.IsKeyDown(Keys.W)) forwards++;
      if (kb.IsKeyDown(Keys.S)) forwards--;
      if (kb.IsKeyDown(Keys.A)) rightwards--;
      if (kb.IsKeyDown(Keys.D)) rightwards++;
      Vector3 current = _pos.ToVec3();
      Vector3 twoD = Front * (1, 0, 1);
      if (twoD != Vector3.Zero)
        twoD.Normalize();
      _velocity += twoD * forwards;
      _velocity += _right * rightwards;
      _velocity.Y -= 0.2f;
      current += _velocity;
      float height = World.HeightAt((_pos.X, _pos.Z));
      if (current.Y < height)
      {
        current.Y = height;
        _velocity.Y = 0;
      }

      _velocity.Xz *= 0.5f;
      _pos.SetVec3(current);
    }

    private void OnMouseMove()
    {
      if (Fall.Instance.CursorState != CursorState.Grabbed || !Fall.Instance.IsFocused)
        return;
      float xPos = Fall.MouseX;
      float yPos = Fall.MouseY;

      if (FirstMouse)
      {
        _lastX = xPos;
        _lastY = yPos;
        FirstMouse = false;
      }

      float xOffset = xPos - _lastX;
      float yOffset = _lastY - yPos;
      _lastX = xPos;
      _lastY = yPos;

      const float SENSITIVITY = 0.1f;
      xOffset *= SENSITIVITY;
      yOffset *= SENSITIVITY;

      _pos.Yaw += xOffset;
      _pos.Pitch += yOffset;

      if (_pos.Pitch > 89.0f)
        _pos.Pitch = 89.0f;
      if (_pos.Pitch < -89.0f)
        _pos.Pitch = -89.0f;
    }

    public Vector3 Eye()
    {
      if (Fall.FirstPerson)
      {
        return _pos.ToLerpedVec3() + (0, 5, 0);
      }

      Vector3 ret = Target() - Front * (Fall.FarCamera ? 625f : 25f);
      if (!Fall.FarCamera)
        ret.Y = MathF.Max(ret.Y, World.HeightAt((ret.X, ret.Z)) + 0.33f);
      return ret;
    }
    
    public Vector3 Target()
    {
      if (Fall.FirstPerson)
      {
        return Eye() + Front;
      }
      else
      {
        return _pos.ToLerpedVec3() + (0, 4, 0);
      }
    }

    public Matrix4 GetCameraMatrix()
    {
      if (_pos == null)
        return Matrix4.Identity;
      if (Fall.FirstPerson)
      {
        Vector3 eye = Eye();
        return Matrix4.LookAt(eye, eye + Front, _up);
      }

      {
        Matrix4 lookAt = Matrix4.LookAt(Eye(), Target(), _up);
        return lookAt;
      }
    }
  }
}