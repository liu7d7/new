namespace New.Shared.Components;

public enum CompType
{
  Camera,
  Collision,
  Model3D,
  Play,
  Snow,
  Tree,
  Drop,
  ItemDrop,
  Inventory,
  Life,
  Size
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

  public virtual void Interact(Interaction interaction)
  { }

  public virtual void Die()
  { }

  public override int GetHashCode()
  {
    return GetType().GetHashCode();
  }
}