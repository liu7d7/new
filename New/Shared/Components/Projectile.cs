using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public class Projectile : Entity.Component
  {
    private static readonly Model3d _model;
    private readonly Vector3 _dir;
    private readonly float _speed;

    static Projectile()
    {
      _model = Model3d.Read("icosphere", new Dictionary<string, uint>());
      _model.Scale(0.5f);
    }

    public Projectile(Vector3 dir, float speed) : base(Entity.CompType.Projectile)
    {
      _dir = dir;
      _speed = speed;
    }

    public override void Render(Entity objIn)
    {
      base.Render(objIn);

      _model.Render(objIn.LerpedPos);
    }

    public override void Update(Entity objIn)
    {
      base.Update(objIn);

      FloatPos pos = FloatPos.Get(objIn);
      pos.SetPrev();
      pos.IncVec3(_dir * _speed);
    }
  }
}