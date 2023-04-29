using New.Engine;
using New.Shared.Components;
using OpenTK.Mathematics;

namespace New.Shared;

public class Entity
{
  public readonly int Index;
  private Memory<Component> _components;
  public readonly long Id = Rand.NextLong();
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

  public float LerpedX => Maths.Lerp(PrevX, X, Fall.TickDelta);
  public float LerpedY => Maths.Lerp(PrevY, Y, Fall.TickDelta);
  public float LerpedZ => Maths.Lerp(PrevZ, Z, Fall.TickDelta);
  public float LerpedYaw => Maths.Lerp(PrevYaw, Yaw, Fall.TickDelta);
  public float LerpedPitch => Maths.Lerp(PrevPitch, Pitch, Fall.TickDelta);

  public Vector3 Pos => ToVec3();
  public Vector3 LerpedPos => ToLerpedVec3();

  public const float FALL_ACCEL = 0.2f;

  public Entity()
  {
    X = PrevX = Y = PrevY = Z = PrevZ = Yaw = PrevYaw = Pitch = PrevPitch = 0;
    Index = ComponentPool.Rent();
    _components = ComponentPool.Get(Index);
  }

  public void Relocate()
  {
    _components = ComponentPool.Get(Index);
  }

  public void Update()
  {
    Span<Component> c = _components.Span;
    for (int i = 0; i < _components.Length; i++) c[i]?.Update();
  }

  public void Render()
  {
    Span<Component> c = _components.Span;
    for (int i = 0; i < _components.Length; i++) c[i]?.Render();
  }

  public void Interact(Interaction interaction)
  {
    Span<Component> c = _components.Span;
    for (int i = 0; i < _components.Length; i++) c[i]?.Interact(interaction);
  }
  
  public void Die()
  {
    Span<Component> c = _components.Span;
    for (int i = 0; i < _components.Length; i++) c[i]?.Die();
    ComponentPool.Ret(Index);
  }

  public void Add(Component component)
  {
    component.Me = this;
    _components.Span[(int)component.Type] = component;
  }

  public T Get<T>(CompType t) where T : Component
  {
    return (T)_components.Span[(int)t];
  }

  public IPosProvider GetMesh()
  {
    Span<Component> c = _components.Span;
    for (int i = 0; i < _components.Length; i++)
    {
      if (c[i] is IMeshSupplier meshSupplier)
        return meshSupplier.Mesh;
    }

    return null;
  }

  public bool Has(CompType t)
  {
    return _components.Span[(int)t] != null;
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
    return unchecked((int)Id) ^ (int)(Id >> 32);
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

  public Vector2i ChunkPos => new((int)X >> 4, (int)Z >> 4);
}