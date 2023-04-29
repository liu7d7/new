namespace New.Shared.Items;

public sealed class ItemStack
{
  public uint Size;
  public readonly Item Item;
  
  public ItemStack(Item item, uint size = 1)
  {
    Item = item;
    Size = size;
  }

  public ItemStack() : this(Item.AIR, 0) 
  { }
  
  public static bool operator ==(ItemStack lhs, ItemStack rhs)
  {
    return lhs?.Item == rhs?.Item && lhs?.Size == rhs?.Size;
  }
  
  public static bool operator !=(ItemStack lhs, ItemStack rhs)
  {
    return !(lhs == rhs);
  }
  
  public override bool Equals(object obj)
  {
    return obj is ItemStack stack && this == stack;
  }
  
  public override int GetHashCode()
  {
    return HashCode.Combine(Item, Size);
  }
}