using New.Engine;
using OpenTK.Mathematics;

namespace New.Shared
{
  public static class ComponentPool
  {
    private static readonly Vec<Component> _components = new(524288);
    private static readonly Queue<int> _holes = new();
    public static int Count { get; private set; }
    public static int TheoreticalCount => _components.Capacity / _size;

    private const int _size = (int)CompType.Size;

    public static int Rent()
    {
      if (_holes.Count > 0)
      {
        int index = _holes.Dequeue();
        Array.Fill(_components.Items, null, index * _size, _size);
        return index;
      }

      Count++;
      _components.Count = (Count - 1) * _size;
      if (!_components.EnsureCapacity(Count * _size)) return Count - 1;

      foreach (Entity obj in Fall.World.Objs)
      {
        obj.Relocate();
      }

      Console.WriteLine($"relocation needed @ {_components.Capacity}");
      return Count - 1;
    }

    public static void Return(int index)
    {
      _holes.Enqueue(index);
    }

    public static Memory<Component> Get(int index)
    {
      return _components.Items.AsMemory(index * _size, _size);
    }
  }

  public class Entity
  {
    public readonly int Index;
    private Memory<Component> _components;
    public readonly long Id = Rand.NextLong();
    public bool Removed;
    public bool Updates;
    public int Life = 5;

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

    public void Collide(Entity other)
    {
      Span<Component> c = _components.Span;
      for (int i = 0; i < _components.Length; i++) c[i]?.Collide(other);
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

    public void TakeDamage(int damage)
    {
      Life -= damage;
      if (Life <= 0) Removed = true;
    }

    public Vector2i ChunkPos => new((int)X >> 4, (int)Z >> 4);
  }

  public class Component
  {
    public readonly CompType Type;
    public Entity Me;

    protected Component(CompType type)
    {
      Type = type;
    }

    public virtual void Update()
    { }

    public virtual void Render()
    { }

    public virtual void Collide(Entity other)
    { }

    public override int GetHashCode()
    {
      return GetType().GetHashCode();
    }
  }

  public enum CompType
  {
    Camera,
    Collision,
    Model3D,
    Player,
    Snow,
    Tree,
    Size
  }
}