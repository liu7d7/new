using New.Engine;
using New.Shared.Worlds.World;
using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public class Snow : Component
  {
    public const int MODEL_COUNT = 2;
    private static readonly Model3d[] _snow;
    private static int _modelIndexCount;

    private readonly Vector3 _dir = new(Rand.NextFloat(-0.7f, 0.7f), Rand.NextFloat(-0.6f, -0.2f),
      Rand.NextFloat(-0.7f, 0.7f));

    private readonly int _modelIndex;

    private readonly int _mul = Rand.Next(0, 2) == 1 ? -1 : 1;

    private readonly int _offset = Rand.Next(0, 360);

    private float _landing = float.MaxValue;

    static Snow()
    {
      _snow = new Model3d[MODEL_COUNT];
      for (int i = 0; i < MODEL_COUNT; i++)
      {
        _snow[i] = Model3d.Read("snow", new Dictionary<string, uint>
          { { "hi", (uint)i } });
        _snow[i].Scale(0.5f);
      }
    }

    public Snow() : base(CompType.Snow)
    {
      _modelIndexCount++;
      _modelIndex = _modelIndexCount % MODEL_COUNT;
    }

    public override void Render()
    {
      base.Render();

      RenderSystem.Push();
      RenderSystem.Translate(-Me.LerpedX, -Me.LerpedY, -Me.LerpedZ);
      RenderSystem.Rotate(Fall.Now / 2f % 360 * _mul, 0.5f, 1, 0.5f);
      RenderSystem.Scale(MathHelper.Clamp(1 - (Fall.Now - _landing) / 1000f, 0, 1));
      RenderSystem.Translate(Me.LerpedX, Me.LerpedY, Me.LerpedZ);
      _snow[_modelIndex].Render(Me.LerpedX, Me.LerpedY, Me.LerpedZ);
      RenderSystem.Pop();
    }

    public override void Update()
    {
      base.Update();

      Me.SetPrev();

      if (Me.Y > World.HeightAt((Me.X, Me.Z)) - 0.5f)
      {
        float x = ((Fall.Now + _offset) / 3f % 360f).Rad();
        Me.X += _dir.X * (MathF.Sin(x * 0.5f) / 4f + 1.5f);
        Me.Y += _dir.Y * (MathF.Sin(x * 1.6f) * MathF.Sin(x * 1.3f) * MathF.Sin(x * 0.7f)) - 0.1f;
        Me.Z += _dir.Z * (MathF.Cos(x * 0.5f) / 4f + 1.5f);
      }
      else if (MathF.Abs(_landing - float.MaxValue) < 0.3f)
      {
        _landing = Fall.Now;
      }

      if (Fall.Now - _landing > 1000) Me.Removed = true;
    }
  }
}