namespace New.Shared.Worlds;

public static class Biome
{
  public static float Height(int x, int y)
  {
    float zeroToOne = SimplexNoise.Noise.CalcPixel2D(x, y, 0.0005f) / 255f * 1.2f;
    float transformed = 0.5f * MathF.Pow(2 * zeroToOne - 1, 3) + 0.5f;
    return transformed * 1.4f - 0.4f;
  }

  public static float Variance(int x, int y)
  {
    return MathF.Abs(SimplexNoise.Noise.CalcPixel2D(x, y, 0.0075f) / 255f - 0.5f) * 2 * MathF.Abs(Height(x, y) - 0.5f);
  }
}