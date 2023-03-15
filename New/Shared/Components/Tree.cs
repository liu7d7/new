namespace New.Shared.Components
{
  public class Tree : Component
  {
    public Tree() : base(CompType.Tree)
    {
    }

    public override void Update(Entity objIn)
    {
      base.Update(objIn);

      if (Fall.InView > Snow.MODEL_COUNT * 400) return;

      Entity obj = new()
      {
        Updates = true
      };
      obj.Add(new Snow());
      obj.X = objIn.Pos.X;
      obj.Y = objIn.Pos.Y + 16 + Rand.NextFloat(0, 12);
      obj.Z = objIn.Pos.Z;
      Fall.World.Objs.Add(obj);
    }
  }
}