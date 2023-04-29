namespace New.Shared.Components;

public class Life : Component
{
  private int _health;
  
  public int Health
  {
    get => _health;
    private set
    {
      _health = value;
      if (_health < 0) Me.Removed = true;
    }
  }
  
  public Life(int health) : base(CompType.Life)
  {
    Health = health;
  }

  public override void Interact(Interaction interaction)
  {
    base.Interact(interaction);
    
    if (interaction.Type != InteractType.Hit) return;
    
    Health--;
  }
}