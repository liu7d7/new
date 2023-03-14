namespace New.Shared.Components
{
  public class Collision : Entity.Component
  {
    public List<Box> Boxes;

    public Collision(List<Box> boxes) : base(Entity.CompType.Collision)
    {
      Boxes = boxes;
    }
  }
}