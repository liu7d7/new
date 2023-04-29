using New.Engine;
using New.Shared.Items;

namespace New.Shared.Components;

public sealed class Drop : Component
{
  public readonly Item Item;
  public readonly uint Min;
  public readonly uint Max;

  public Drop(Item item, uint min, uint max) : base(CompType.Drop)
  {
    Item = item;
    Min = min;
    Max = max;
  }

  public override void Die()
  {
    base.Die();

    Entity entity = new()
    {
      Updates = true,
      X = Me.X,
      Y = Me.Y + 4,
      Z = Me.Z
    };
    entity.SetPrev();
    entity.Add(new ItemDrop(new ItemStack(Item, Rand.NextUInt(Min, Max))));
    entity.Add(new Model(Item.Model, 0));
    entity.Add(new MeshCollision<PC>(Model.Get(entity).Model3d.Mesh));
    Fall.World.Add(entity);
  }
}