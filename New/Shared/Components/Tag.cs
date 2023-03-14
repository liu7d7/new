namespace New.Shared.Components
{
  public class Tag : Entity.Component
  {
    public int Id;
    public string Name;

    public Tag(int id, string name = "") : base(CompType.Tag)
    {
      Id = id;
      Name = name;
    }
  }
}