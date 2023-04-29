using New.Engine;
using OpenTK.Mathematics;

namespace New.Shared.Components;

public abstract class Collision : Component
{
  protected Collision() : base(CompType.Collision)
  { }

  public abstract bool ray_collides(Vector3 offset, Vector3 ro, Vector3 rd, out float dist);
}

public sealed class MeshCollision<T> : Collision, IMeshSupplier where T : struct, IPos3d
{
  private readonly Mesh<T> _mesh;
  public IPosProvider Mesh => _mesh;

  public MeshCollision(Mesh<T> mesh)
  {
    _mesh = mesh;
  }

  public override bool ray_collides(Vector3 offset, Vector3 ro, Vector3 rd, out float dist)
  {
    Span<(int, int, int)> tris = _mesh.Tris.Items;
    Span<T> verts = _mesh.Vbo.Vertices;
    ro.X -= offset.X;
    ro.Y -= offset.Y;
    ro.Z -= offset.Z;
    for (int i = 0; i < tris.Length; i++)
    {
      Vector3 v0 = verts[tris[i].Item1].Pos, v1 = verts[tris[i].Item2].Pos, v2 = verts[tris[i].Item3].Pos;

      Vector3 e1 = v1 - v0;
      Vector3 e2 = v2 - v0;

      Maths.Cross(rd, e2, out Vector3 h);
      float a = Maths.Dot(e1, h);

      if (a is > -0.00001f and < 0.00001f)
        continue;

      float f = 1.0f / a;
      Vector3 s = ro - v0;
      float u = f * Maths.Dot(s, h);

      if (u is < 0.0f or > 1.0f)
        continue;

      Vector3.Cross(s, e1, out Vector3 q);
      float v = f * Maths.Dot(rd, q);

      if (v is < 0.0f or > 1.0f)
        continue;

      float t = f * Maths.Dot(Maths.Cross(s, e1), e2);

      if (t <= 0.00001f) continue;

      dist = t;
      return true;
    }

    dist = float.MaxValue;
    return false;
  }
}