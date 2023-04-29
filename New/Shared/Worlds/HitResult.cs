using OpenTK.Mathematics;

namespace New.Shared.Worlds;

public enum HitResultType
{
  None,
  Entity,
  Tile
}

public struct HitResult
{
  public readonly Vector3 Hit;
  public readonly HitResultType Type;
  public readonly Entity Entity;
  public readonly Chunk Chunk;
  public readonly float Distance;

  public HitResult(HitResultType type = HitResultType.None, float dist = 0, float hitX = 0, float hitY = 0, float hitZ = 0,
    Entity entity = null, Chunk chunk = null)
  {
    Hit = (hitX, hitY, hitZ);
    Type = type;
    Entity = entity;
    Chunk = chunk;
  }
}