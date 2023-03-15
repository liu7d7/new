using New.Engine;
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
        _snow[i] = Model3d.Read("snow", new Dictionary<string, uint> { { "hi", (uint)i } });
        _snow[i].Scale(0.5f);
      }
    }

    public Snow() : base(CompType.Snow)
    {
      _modelIndexCount++;
      _modelIndex = _modelIndexCount % MODEL_COUNT;
    }

    public override void Render(Entity objIn)
    {
      base.Render(objIn);

      RenderSystem.Push();
      RenderSystem.Translate(-objIn.LerpedPos);
      RenderSystem.Rotate(Environment.TickCount / 2f % 360 * _mul, 0.5f, 1, 0.5f);
      RenderSystem.Scale(MathHelper.Clamp(1 - (Environment.TickCount - _landing) / 1000f, 0, 1));
      RenderSystem.Translate(objIn.LerpedPos);
      _snow[_modelIndex].Render(objIn.LerpedPos);
      RenderSystem.Pop();
    }

    public override void Update(Entity objIn)
    {
      base.Update(objIn);

      FloatPos pos = FloatPos.Get(objIn);
      pos.SetPrev();

      if (pos.Y > World.HeightAt((pos.X, pos.Z)) - 0.5f)
      {
        float x = ((Environment.TickCount + _offset) / 3f % 360f).Rad();
        pos.X += _dir.X * (MathF.Sin(x * 0.5f) / 4f + 1.5f);
        pos.Y += _dir.Y * (MathF.Sin(x * 1.6f) * MathF.Sin(x * 1.3f) * MathF.Sin(x * 0.7f)) - 0.1f;
        pos.Z += _dir.Z * (MathF.Cos(x * 0.5f) / 4f + 1.5f);
      }
      else if (System.Math.Abs(_landing - float.MaxValue) < 0.3f)
      {
        _landing = Environment.TickCount;
      }

      if (Environment.TickCount - _landing > 1000) objIn.Removed = true;
    }
  }
}