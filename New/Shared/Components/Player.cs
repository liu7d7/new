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
    private const float _punchAnimLength = 300;

    private float HandAnimation(float time, float duration)
    {
      float x = time / duration;
      return -MathF.Pow(2 * x - 1, 2) + 1;
    }

    private float Animate(float p)
    {
      return Fall.Now - p is > 0 and < _punchAnimLength ? HandAnimation(Fall.Now - p, _punchAnimLength) : 0;
    }

    static Player()
    {
      _head = Model3d.Read("icosphere", new Dictionary<string, uint>());
      _hands = Model3d.Read("icosphere", new Dictionary<string, uint>
        { { "fortnite", Colors.NextColor().ToUint() } });
      _capeShader = new Shader("cape", "basic");
    }

    public Player() : base(CompType.Player)
    {
      _color = Colors.NextColor();
      _cape = new Mesh<P>(DrawMode.TRIANGLE, _capeShader, true);
      _cape.Begin();
      const int SEGMENTS = 16;
      const float SEG_HEIGHT = 0.33f;
      for (float y = 6f; y > 1.66f; y -= SEG_HEIGHT)
      for (int x = -SEGMENTS; x < SEGMENTS; x++)
        _cape.Quad(
          _cape.Put(new(x, y)).Next(),
          _cape.Put(new(x + 1, y)).Next(),
          _cape.Put(new(x + 1, y - SEG_HEIGHT)).Next(),
          _cape.Put(new(x, y - SEG_HEIGHT)).Next()
        );

      _cape.End();
    }

    public override void Update()
    {
      base.Update();

      if (Fall.Instance.MouseState.IsButtonDown(MouseButton.Left) && Fall.Now - _rightp > _punchAnimLength &&
          Fall.Now - _leftp > _punchAnimLength)
      {
        _leftp = Fall.Now;
        Fall.HitResult.Entity?.TakeDamage(1);
      }
      else if (Fall.Instance.MouseState.IsButtonDown(MouseButton.Right) && Fall.Now - _rightp > _punchAnimLength &&
               Fall.Now - _leftp > _punchAnimLength)
      {
        _rightp = Fall.Now;
        Fall.HitResult.Entity?.TakeDamage(1);
      }
    }

    public override void Render()
    {
      base.Render();

      if (Fall.FirstPerson)
      {
        RenderFirstPerson();
      }
      else
      {
        RenderThirdPerson();
      }
    }

    private static readonly Vector3 _headOffset = (0f, 4.5f, 0f);

    private void RenderFirstPerson()
    {
      Vector3 renderPos = Me.LerpedPos;
      float lyaw = Me.LerpedYaw + 180;

      RenderHand(lyaw, _leftp, 1, renderPos, (1.2f, 4.75f, 1.2f), (1.15f, 0f, 1.15f), 0.13f, 22.5f, true);
      RenderHand(lyaw, _rightp, -1, renderPos, (1.2f, 4.75f, 1.2f), (1.15f, 0f, 1.15f), 0.13f, 22.5f, true);
    }

    private void RenderThirdPerson()
    {
      Vector3 renderPos = Me.LerpedPos;

      RenderSystem.Push();
      RenderSystem.Translate(-renderPos - _headOffset);
      RenderSystem.Scale(0.66f);
      RenderSystem.Translate(renderPos + _headOffset);
      _head.Render(renderPos + _headOffset);
      RenderSystem.Pop();

      float lyaw = Me.LerpedYaw + 180;

      RenderHand(lyaw, _leftp, 1, renderPos, (1f, 3f, 1f), (1.75f, 0f, 1.75f), 0.33f, 80);
      RenderHand(lyaw, _rightp, -1, renderPos, (1f, 3f, 1f), (1.75f, 0f, 1.75f), 0.33f, 80);

      float yawAdd = (Animate(_leftp) - Animate(_rightp)) * 20;
      lyaw += yawAdd;

      bool cull = RenderSystem.Culling;
      RenderSystem.Culling = false;
      _capeShader.Bind();
      _capeShader.SetFloat("_yaw", lyaw);
      _capeShader.SetVector3("_translation", renderPos + (-0.15f * MathF.Cos(lyaw.Rad()), -2, -0.15f * MathF.Sin(lyaw.Rad())));
      _capeShader.SetVector4("_color", _color.ToVector4());
      _cape.Render();
      RenderSystem.Culling = cull;
    }

    private void RenderHand(float lyaw, float progress, int side, Vector3 renderPos, Vector3 handBaseOffset, Vector3 handFullOffset,
      float scale, float handBaseOff, bool fp = false)
    {
      float prog = Fall.Now - progress is > 0 and < _punchAnimLength
        ? HandAnimation((float)GLFW.GetTime() * 1000f - progress, _punchAnimLength)
        : 0;
      Vector3 rotation = (MathF.Cos((lyaw + 180 - handBaseOff * side).Rad()), 1f, MathF.Sin((lyaw + 180 - handBaseOff * side).Rad()));
      Vector3 handRotation = (MathF.Cos((lyaw + 180 + 20 * prog * side).Rad()), 1f, MathF.Sin((lyaw + 180 + 20 * prog * side).Rad()));
      Vector3 handRenderPos = renderPos + handBaseOffset * rotation + handFullOffset * prog * handRotation;
      RenderSystem.Push();
      RenderSystem.Translate(-handRenderPos);
      RenderSystem.Scale(scale);
      RenderSystem.Translate(handRenderPos);
      if (fp)
      {
        Vector3 head = Camera.Get(Me).Eye();
        RenderSystem.Translate(-head);
        RenderSystem.Rotate(Me.LerpedPitch, Camera.Get(Me).Right);
        RenderSystem.Translate(head);
      }

      _hands.Render(handRenderPos);
      RenderSystem.Pop();
    }
  }
}