using New.Shared.Items;

namespace New.Shared.Components;

public sealed class Inventory : Component
{
  private readonly List<ItemStack> _items = new();

  public Inventory() : base(CompType.Inventory)
  { }
  
  public static Inventory Get(Entity entity)
  {
    return entity.Get<Inventory>(CompType.Inventory);
  }
  
  public void Add(ItemStack stack)
  {
    ItemStack item = _items.Find(i => i.Item == stack.Item);
    if (item == null)
    {
      _items.Add(stack);
      return;
    }
    item.Size += stack.Size;
  }
}