using System.Buffers;
using New.Engine;
using New.Shared.Components;
using OpenTK.Mathematics;

namespace New.Shared
{
  public class Entity
  {
    public readonly Component[] Components = ArrayPool<Component>.Shared.Rent((int)CompType.Size);
    public readonly Guid Id = Guid.NewGuid();
    public bool Removed;
    public bool Updates;

    public Vector3 Pos => Has(CompType.FloatPos) ? FloatPos.Get(this).ToVec3() : FloatPosStatic.Get(this).ToVec3();

    public Vector3 LerpedPos =>
      Has(CompType.FloatPos) ? FloatPos.Get(this).ToLerpedVec3() : FloatPosStatic.Get(this).ToVec3();

    public void Update()
    {
      Span<Component> c = Components;
      for (int i = 0; i < Components.Length; i++) c[i]?.Update(this);
    }

    public void Render()
    {
      Span<Component> c = Components;
      for (int i = 0; i < Components.Length; i++) c[i]?.Render(this);
    }

    public void Collide(Entity other)
    {
      Span<Component> c = Components;
      for (int i = 0; i < Components.Length; i++) c[i]?.Collide(this, other);
    }

    public void Add(Component component)
    {
      Components[(int)component.Type] = component;
    }

    public T Get<T>(CompType t) where T : Component
    {
      return (T)Components[(int)t];
    }
    
    public IPosProvider GetMesh()
    {
      for (int i = 0; i < Components.Length; i++)
      {
        if (Components[i] is IMeshSupplier meshSupplier)
          return meshSupplier.Mesh;
      }

      return null;
    }

    public bool Has(CompType t)
    {
      return Components[(int)t] != null;
    }

    public static bool operator ==(Entity one, Entity two)
    {
      return one?.Equals(two) ?? false;
    }

    public static bool operator !=(Entity one, Entity two)
    {
      return !(one == two);
    }

    public override bool Equals(object obj)
    {
      return obj is Entity other && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }
  }

  public class Component
  {
    public readonly CompType Type;

    protected Component(CompType type)
    {
      Type = type;
    }

    public virtual void Update(Entity objIn)
    {
    }

    public virtual void Render(Entity objIn)
    {
    }

    public virtual void Collide(Entity objIn, Entity other)
    {
    }

    public override int GetHashCode()
    {
      return GetType().GetHashCode();
    }
  }

  public enum CompType
  {
    NotAType,
    Camera,
    Collision,
    FloatPos,
    Model3D,
    Player,
    Snow,
    Tag,
    Tree,
    Projectile,
    FloatPosStatic,
    Size
  }
}