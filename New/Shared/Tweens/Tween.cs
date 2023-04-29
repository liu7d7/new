using OpenTK.Mathematics;

namespace New.Shared.Tweens;

public sealed class Tween : BaseTween
{
  private readonly Animations.Animation _animation;

  public Tween(Animations.Animation animation, float duration, bool repeating)
  {
    LastActivation = Fall.Now;
    _animation = animation;
    Infinite = repeating;
    Duration = duration;
  }

  public override float Output()
  {
    return MathHelper.Clamp(Infinite
        ? _animation(Duration, (Fall.Now - LastActivation) % Duration)
        : _animation(Duration, Fall.Now - LastActivation), 0, 1);
  }

  public override float output_at(float time)
  {
    if (time < LastActivation) return 0;

    if (time > LastActivation + Duration && !Infinite) return 1;

    return MathHelper.Clamp(Infinite
        ? _animation(Duration, (time - LastActivation) % Duration)
        : _animation(Duration, time - LastActivation), 0, 1);
  }

  public override bool Done()
  {
    return Fall.Now - LastActivation > Duration;
  }
}