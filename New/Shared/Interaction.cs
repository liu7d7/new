namespace New.Shared;

public enum InteractType
{
  Hit,
  Use,
  Pickup
}

public enum Hand
{
  Left, 
  Right
}

public sealed class Interaction
{
  public readonly Entity Src;
  public readonly InteractType Type;
  public readonly Hand Hand;

  public Interaction(Entity src, Hand hand, InteractType type)
  {
    Src = src;
    Type = type;
    Hand = hand;
  }
}
