namespace New.Shared.Worlds.World
{
  public class Biome
  {
    public static float Height(int x, int y)
    {
      float zeroToOne = SimplexNoise.Noise.CalcPixel2D(x, y, 0.0005f) / 255f * 1.2f;
      float transformed = 0.5f * MathF.Pow(2.3f * zeroToOne - 1.33f, 5) + 0.6f;
      return transformed * 1.6f - 0.6f;
    }

    public static float Variance(int x, int y)
    {
      return MathF.Pow(MathF.Abs(SimplexNoise.Noise.CalcPixel2D(x, y, 0.0075f) / 255f - 0.5f) * 2 * MathF.Abs(Height(x, y) + 0.4f),
        1.3f);
    }
  }
}