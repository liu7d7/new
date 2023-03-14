using System.Buffers;
using OpenTK.Graphics.OpenGL4;

namespace New.Engine
{
  public class Ibo
  {
    private readonly int _handle;
    private readonly bool _static;
    private int _count;
    private int[] _indices;

    public Ibo(int initialCapacity, bool @static)
    {
      _handle = GL.GenBuffer();
      _indices = ArrayPool<int>.Shared.Rent(initialCapacity);
      _static = @static;
    }

    public void Bind()
    {
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _handle);
    }

    public static void Unbind()
    {
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Put(int element)
    {
      if (_count + 1 > _indices.Length)
      {
        ArrayPool<int>.Shared.Return(_indices);
        int[] prev = _indices;
        _indices = ArrayPool<int>.Shared.Rent(_indices.Length * 2);
        Array.Copy(prev, _indices, _count);
      }

      _indices[_count] = element;
      _count++;
    }

    public void Upload(bool unbindAfter = true)
    {
      Bind();
      GL.BufferData(BufferTarget.ElementArrayBuffer, _count * sizeof(int), _indices,
        _static ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw);
      if (unbindAfter)
        Unbind();
    }

    public void Clear()
    {
      _count = 0;
    }
  }
}