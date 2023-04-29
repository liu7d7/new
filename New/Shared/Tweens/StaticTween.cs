namespace New.Shared.Tweens;

public sealed class StaticTween : BaseTween
{
  private readonly float _output;

  public StaticTween(float output, float duration)
  {
    _output = output;
    Duration = duration;
  }

  public override float Output()
  {
    return _output;
  }

  public override float output_at(float time)
  {
    return _output;
  }

  public override bool Done()
  {
    return Fall.Now - LastActivation > Duration;
  }
}