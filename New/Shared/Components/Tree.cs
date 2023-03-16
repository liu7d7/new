namespace New.Shared.Components
{
  public class Tree : Component
  {
    public Tree() : base(CompType.Tree)
    {
    }

    public override void Update()
    {
      base.Update();

      if (Fall.InView > Snow.MODEL_COUNT * 400) return;

      Entity obj = new()
      {
        Updates = true
      };
      obj.Add(new Snow());
      obj.X = Me.X;
      obj.Y = Me.Y + 16 + Rand.NextFloat(0, 12);
      obj.Z = Me.Z;
      obj.SetPrev();
      Fall.World.ObjsToAdd.Add(obj);
    }
  }
}