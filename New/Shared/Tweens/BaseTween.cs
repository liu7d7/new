namespace New.Shared.Tweens
{
  public abstract class BaseTween
  {
    public float Duration;
    public bool Infinite = false;
    public float LastActivation = Fall.Now;

    public abstract float Output();
    public abstract float OutputAt(float time);
    public abstract bool Done();
  }
}