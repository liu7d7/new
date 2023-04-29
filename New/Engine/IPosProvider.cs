using OpenTK.Mathematics;

namespace New.Engine;

public interface IPosProvider
{
  public (int, Vector3) closest_vertex(Vector3 pos);
  public void set_pos(int index, Vector3 pos);
  public void End();
}