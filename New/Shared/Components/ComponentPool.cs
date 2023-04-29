namespace New.Shared.Components;

public static class ComponentPool
{
  private static readonly Vec<Component> _components = new(1 << 19);
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

  public static void Ret(int index)
  {
    _holes.Enqueue(index);
  }

  public static Memory<Component> Get(int index)
  {
    return _components.Items.AsMemory(index * _size, _size);
  }
}