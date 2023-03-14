using OpenTK.Mathematics;

namespace New.Engine
{
  public interface IPosProvider
  {
    public (int, Vector3) ClosestVertex(Vector3 pos);
    public void SetPos(int index, Vector3 pos);
    public void End();
  }
}