﻿using New.Engine;
using OpenTK.Mathematics;

namespace New.Shared.Components
{
  public abstract class Collision : Entity.Component
  {
    protected Collision() : base(CompType.Collision)
    {
    }

    public abstract bool RayCollides(Vector3 offset, Vector3 ro, Vector3 rd, out float dist);
  }
  
  public class MeshCollision<T> : Collision, IMeshSupplier where T : struct, IPos3d
  {
    private readonly Mesh<T> _mesh;
    public IPosProvider Mesh => _mesh;

    public MeshCollision(Mesh<T> mesh)
    {
      _mesh = mesh;
    }

    public override bool RayCollides(Vector3 offset, Vector3 ro, Vector3 rd, out float dist)
    {
      Span<(int, int, int)> tris = _mesh.Tris.Items;
      Span<T> verts = _mesh.Vbo.Vertices;
      for (int i = 0; i < tris.Length; i++)
      {
        Vector3 v0 = verts[tris[i].Item1].Pos + offset, v1 = verts[tris[i].Item2].Pos + offset, v2 = verts[tris[i].Item3].Pos + offset;
        
        Vector3 e1 = v1 - v0;
        Vector3 e2 = v2 - v0;
        
        Vector3 h = Vector3.Cross(rd, e2);
        float a = Vector3.Dot(e1, h);
        
        if (a is > -0.00001f and < 0.00001f)
          continue;
        
        float f = 1.0f / a;
        Vector3 s = ro - v0;
        float u = f * Vector3.Dot(s, h);
        
        if (u is < 0.0f or > 1.0f)
          continue;
        
        Vector3 q = Vector3.Cross(s, e1);
        float v = f * Vector3.Dot(rd, q);
        
        if (v is < 0.0f or > 1.0f)
          continue;
        
        float t = f * Vector3.Dot(Vector3.Cross(s, e1), e2);

        if (!(t > 0.00001f)) continue;
        
        dist = t;
        return true;
      }
      
      dist = float.MaxValue;
      return false;
    }
  }
}