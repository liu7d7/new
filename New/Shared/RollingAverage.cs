namespace New.Shared;

public sealed class RollingAvg
{
  private readonly int _size;
  public readonly Queue<double> Values = new();
  private double _sum;

  public RollingAvg(int size)
  {
    _size = size;
  }

  public double Average => _sum / Values.Count;

  public void Add(double value)
  {
    _sum += value;
    Values.Enqueue(value);
    if (Values.Count <= _size) return;
    _sum -= Values.Dequeue();
  }
}