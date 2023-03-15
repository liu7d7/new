using New.Engine;
using New.Shared;
using New.Shared.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New.Shared.Components
{
  public class Player : Component
  {
    private static readonly Model3d _head;
    private static readonly Model3d _hands;
    private static readonly Shader _capeShader;

    private readonly Color4 _color;
    private readonly Mesh<P> _cape;

    private float _leftp, _rightp;

    private float HandAnimation(float time, float duration)
    {
      float x = time / duration;
      return -MathF.Pow(2 * x - 1, 2) + 1;
    }

    private float Animate(float p)
    {
      return Fall.Now - p is > 0 and < 400 ? HandAnimation(Fall.Now - p, 400) : 0;
    }

    static Player()
    {
      _head = Model3d.Read("icosphere", new Dictionary<string, uint>());
      _hands = Model3d.Read("icosphere", new Dictionary<string, uint> { { "fortnite", Colors.NextColor().ToUint() } });
      _capeShader = new Shader("cape", "basic");
    }

    public Player() : base(CompType.Player)
    {
      _color = Colors.NextColor();
      _cape = new Mesh<P>(DrawMode.TRIANGLE, _capeShader, true);
      _cape.Begin();
      const int SEGMENTS = 16;
      for (float y = 6f; y > 1.66f; y -= 0.33f)
      for (int x = -SEGMENTS; x < SEGMENTS; x++)
        _cape.Quad(
          _cape.Put(new(x, y)).Next(),
          _cape.Put(new(x + 1, y)).Next(),
          _cape.Put(new(x + 1, y - 0.5f)).Next(),
          _cape.Put(new(x, y - 0.5f)).Next()
        );

      _cape.End();
    }

    public override void Update(Entity objIn)
    {
      base.Update(objIn);

      if (Fall.Instance.MouseState.IsButtonDown(MouseButton.Left) && Fall.Now - _rightp > 400 && Fall.Now - _leftp > 400)
      {
        _leftp = Fall.Now;
      }
      else if (Fall.Instance.MouseState.IsButtonDown(MouseButton.Right) && Fall.Now - _rightp > 400 && Fall.Now - _leftp > 400)
      {
        _rightp = Fall.Now;
      }
    }

    public override void Render(Entity objIn)
    {
      base.Render(objIn);

      if (Fall.FirstPerson)
      {
        RenderFirstPerson(objIn);
      }
      else
      {
        RenderThirdPerson(objIn);
      }
    }

    private void RenderFirstPerson(Entity objIn)
    {
    }

    private void RenderThirdPerson(Entity objIn)
    {
      Vector3 renderPos = objIn.LerpedPos;

      Vector3 headOffset = (0f, 4.5f, 0f);

      RenderSystem.Push();
      RenderSystem.Translate(-renderPos - headOffset);
      RenderSystem.Scale(0.66f);
      RenderSystem.Translate(renderPos + headOffset);
      _head.Render(renderPos + headOffset);
      RenderSystem.Pop();

      float lyaw = FloatPos.Get(objIn).LerpedYaw + 180;

      RenderHand(lyaw, _leftp, 1, renderPos);
      RenderHand(lyaw, _rightp, -1, renderPos);

      float yawAdd = (Animate(_leftp) - Animate(_rightp)) * 20;
      lyaw += yawAdd;

      _capeShader.Bind();
      _capeShader.SetFloat("_yaw", lyaw);
      _capeShader.SetVector3("_translation", renderPos + (-0.15f * MathF.Cos(lyaw.Rad()), -2, -0.15f * MathF.Sin(lyaw.Rad())));
      _capeShader.SetVector4("_color", _color.ToVector4());
      _cape.Render();
    }

    private void RenderHand(float lyaw, float progress, int side, Vector3 renderPos)
    {
      float prog = Fall.Now - progress is > 0 and < 400 ? HandAnimation((float)GLFW.GetTime() * 1000f - progress, 400) : 0;
      Vector3 handBaseOffset = new Vector3(1f, 3f, 1f);
      Vector3 handFullOffset = (1.75f, 0f, 1.75f);
      Vector3 rotation = (MathF.Cos((lyaw + 180 - 80 * side).Rad()), 1f, MathF.Sin((lyaw + 180 - 80 * side).Rad()));
      Vector3 handRotation = (MathF.Cos((lyaw + 180 + 20 * prog * side).Rad()), 1f, MathF.Sin((lyaw + 180 + 20 * prog * side).Rad()));
      Vector3 handRenderPos = renderPos + handBaseOffset * rotation + handFullOffset * prog * handRotation;
      RenderSystem.Push();
      RenderSystem.Translate(-handRenderPos);
      RenderSystem.Scale(0.33f);
      RenderSystem.Translate(handRenderPos);
      _hands.Render(handRenderPos);
      RenderSystem.Pop();
    }
  }
}