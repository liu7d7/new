using OpenTK.Mathematics;

namespace New.Shared.Tweens;

public sealed class FromToTween : BaseTween
{
  public Animations.Animation Animation;
  public float From;
  public float To;

  public FromToTween(Animations.Animation animation, float from, float to, float duration)
  {
    Animation = animation;
    LastActivation = Fall.Now;
    From = from;
    To = to;
    Duration = duration;
  }

  public override float Output()
  {
    if (Fall.Now < LastActivation) return From;

    if (Fall.Now > LastActivation + Duration) return To;

    return MathHelper.Lerp(From, To, Animation(Duration, Fall.Now - LastActivation));
  }

  public override float output_at(float time)
  {
    if (time < LastActivation) return From;

    if (time > LastActivation + Duration) return To;

    return MathHelper.Lerp(From, To, Animation(Duration, time - LastActivation));
  }

  public override bool Done()
  {
    return Fall.Now - LastActivation > Duration;
  }
}