using New.Shared.Components;

namespace New.Shared
{
  public class Item
  {
    public string Name;
    public Model3d Model;
    public int Id;

    private static int _id;
    private static Vec<Item> _items = new();
    private static Dictionary<string, Item> _nameToItem = new();

    public Item(string name, Model3d model)
    {
      Name = name;
      Model = model;
      Id = _id++;
      _items.Add(this);
      _nameToItem[name] = this;
    }

    public static Item Get(string name)
    {
      return _nameToItem.TryGetValue(name, out Item item) ? item : throw new Exception($"item {name} not found");
    }

    public static Item Get(int id)
    {
      return _items[id];
    }

    public static int Count => _items.Count;
  }
}