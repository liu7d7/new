using New.Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New.Shared.Components
{
  public class Player : Entity.Component
  {
    private static readonly Model3d _icosphere;
    private static readonly Shader _capeShader;
    private static readonly Mesh<P> _cape;

    private readonly Color4 _color;

    static Player()
    {
      _icosphere = Model3d.Read("icosphere", new Dictionary<string, uint> { { "fortnite", 1 } });
      _icosphere.Scale(0.66f);
      _capeShader = new Shader("cape", "basic");

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

    public Player() : base(Entity.CompType.Player)
    {
      _color = Colors.NextColor();
    }

    public override void Update(Entity objIn)
    {
      base.Update(objIn);

      if (!Fall.IsPressed(Keys.Space)) return;
      Entity obj = new();
      FloatPos pos = FloatPos.Get(objIn);
      Camera cam = Camera.Get(objIn);
      obj.Add(new FloatPos
      {
        X = pos.X, Y = pos.Y + 3.25f, Z = pos.Z, PrevX = pos.X, PrevY = pos.Y + 3.25f, PrevZ = pos.Z
      });
      obj.Add(new Projectile(cam.Front, 2f));
      obj.Updates = true;
      Fall.World.Objs.Add(obj);
    }

    public override void Render(Entity objIn)
    {
      base.Render(objIn);
      if (Fall.FirstPerson) return;

      Vector3 renderPos = objIn.LerpedPos;

      Vector3 headOffset = (0f, 4.5f, 0f);
      Vector3 bodyOffset = (0f, 2.5f, 0f);

      _icosphere.Render(renderPos + headOffset);

      float lyaw = FloatPos.Get(objIn).LerpedYaw + 180;

      RenderSystem.Culling = false;
      _capeShader.Bind();
      _capeShader.SetFloat("_yaw", lyaw);
      _capeShader.SetVector3("_translation",
        renderPos + (-0.15f * MathF.Cos(lyaw.Rad()), -2, -0.15f * MathF.Sin(lyaw.Rad())));
      _capeShader.SetVector4("_color", _color.ToVector4());
      _cape.Render();
      RenderSystem.Culling = true;
    }
  }
}