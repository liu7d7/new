using New.Shared.Items;
using New.Shared.Worlds;
using OpenTK.Mathematics;

namespace New.Shared.Components;

public sealed class ItemDrop : Component
{
  public readonly ItemStack Stack;
  private Vector3 _velo;
  
  public ItemDrop(ItemStack stack) : base(CompType.ItemDrop)
  {
    Stack = stack;
    _velo = new Vector3(Rand.NextFloat(-0.5f, 0.5f), Rand.NextFloat(0.5f, 1f), Rand.NextFloat(-0.5f, 0.5f));
  }
  
  public override void Update()
  {
    base.Update();

    Me.SetPrev();
    if (World.HeightAt(Me.X, Me.Z) + _velo.Y + 1.25 > Me.Y) return;

    _velo.Y -= Entity.FALL_ACCEL * 0.5f;
    Me.X += _velo.X;
    Me.Y += _velo.Y;
    Me.Z += _velo.Z;
  }

  public override void Interact(Interaction interaction)
  {
    base.Interact(interaction);

    if (interaction.Type != InteractType.Pickup) return;
    Inventory.Get(interaction.Src).Add(Stack);
    Me.Removed = true;
  }
}