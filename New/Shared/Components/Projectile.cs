using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public class Projectile : Component
  {
    private static readonly Model3d _model;
    private readonly Vector3 _dir;
    private readonly float _speed;

    static Projectile()
    {
      _model = Model3d.Read("icosphere", new Dictionary<string, uint>());
      _model.Scale(0.5f);
    }

    public Projectile(Vector3 dir, float speed) : base(CompType.Projectile)
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
      
      objIn.SetPrev();
      objIn.IncVec3(_dir * _speed);
    }
  }
}