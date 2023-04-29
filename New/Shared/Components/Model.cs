using New.Engine;

namespace New.Shared.Components;

public class Model : Component, IMeshSupplier
{
  public readonly Model3d Model3d;
  public IPosProvider Mesh => Model3d.Mesh;
  private readonly float _rotation;

  public Model(Model3d model3d, float rot) : base(CompType.Model3D)
  {
    Model3d = model3d;
    _rotation = rot;
  }
  
  public static Model Get(Entity entity)
  {
    return entity.Get<Model>(CompType.Model3D);
  }

  public override void Render()
  {
    base.Render();

    RenderSystem.Push();
    RenderSystem.Translate(-Me.LerpedX, -Me.LerpedY, -Me.LerpedZ);
    RenderSystem.Rotate(_rotation, 0, 1, 0);
    RenderSystem.Translate(Me.LerpedX, Me.LerpedY, Me.LerpedZ);
    Model3d.Render(Me.LerpedX, Me.LerpedY, Me.LerpedZ);
    RenderSystem.Pop();
  }
}