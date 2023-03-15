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
    
    public float Pitch;
    public float PrevPitch;
    public float PrevX;
    public float PrevY;
    public float PrevYaw;
    public float PrevZ;
    public float X;
    public float Y;
    public float Yaw;
    public float Z;
    
    public float LerpedX => Math.Lerp(PrevX, X, Fall.TickDelta);
    public float LerpedY => Math.Lerp(PrevY, Y, Fall.TickDelta);
    public float LerpedZ => Math.Lerp(PrevZ, Z, Fall.TickDelta);
    public float LerpedYaw => Math.Lerp(PrevYaw, Yaw, Fall.TickDelta);
    public float LerpedPitch => Math.Lerp(PrevPitch, Pitch, Fall.TickDelta);


    public Vector3 Pos => ToVec3();
    public Vector3 LerpedPos => ToLerpedVec3();

    public Entity()
    {
      X = PrevX = Y = PrevY = Z = PrevZ = Yaw = PrevYaw = Pitch = PrevPitch = 0;
    }

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
    
    public Vector3 ToVec3()
    {
      return (X, Y, Z);
    }

    public void SetVec3(Vector3 pos)
    {
      (X, Y, Z) = pos;
    }

    public void IncVec3(Vector3 inc)
    {
      X += inc.X;
      Y += inc.Y;
      Z += inc.Z;
    }

    public Vector3 ToLerpedVec3(float xOff = 0, float yOff = 0, float zOff = 0)
    {
      return (LerpedX + xOff, LerpedY + yOff, LerpedZ + zOff);
    }

    public void SetPrev()
    {
      PrevX = X;
      PrevY = Y;
      PrevZ = Z;
      PrevYaw = Yaw;
      PrevPitch = Pitch;
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
    Model3D,
    Player,
    Snow,
    Tag,
    Tree,
    Projectile,
    Size
  }
}