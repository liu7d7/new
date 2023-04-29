using New.Shared.Components;

namespace New.Shared.Items;

public sealed class Item
{
  public readonly string Name;
  public readonly Model3d Model;
  public readonly int Id;

  private static int _id;
  private static Vec<Item> _items = new();
  private static Dictionary<string, Item> _nameToItem = new();
  
  public static readonly Item AIR = new("air", null);
  public static readonly Item WOOD = new("wood", Model3d.Read("log"));

  private Item(string name, Model3d model)
  {
    Name = name;
    Model = model;
    Id = _id++;
    _items.Add(this);
    _nameToItem[name] = this;
  }
  
  public static int Count => _items.Count;

  public static Item Get(string name)
  {
    return _nameToItem.TryGetValue(name, out Item item) ? item : throw new Exception($"item {name} not found");
  }

  public static Item Get(int id)
  {
    return _items[id];
  }
  
  public static bool operator ==(Item lhs, Item rhs)
  {
    return lhs?.Id == rhs?.Id;
  }
  
  public static bool operator !=(Item lhs, Item rhs)
  {
    return !(lhs == rhs);
  }
  
  public override bool Equals(object obj)
  {
    if (obj is Item item)
      return item == this;
    return false;
  }
  
  public override int GetHashCode()
  {
    return Id;
  }
}