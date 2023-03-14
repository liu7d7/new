using System.Buffers;
using OpenTK.Graphics.OpenGL4;

namespace New.Engine
{
  public class Ibo
  {
    private readonly int _handle;
    private readonly bool _static;
    private int _count;
    private uint[] _indices;

    public Ibo(int initialCapacity, bool @static)
    {
      _handle = GL.GenBuffer();
      _indices = ArrayPool<uint>.Shared.Rent(initialCapacity);
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

    public void Put(uint element)
    {
      if (_count + 1 > _indices.Length)
      {
        ArrayPool<uint>.Shared.Return(_indices);
        uint[] prev = _indices;
        _indices = ArrayPool<uint>.Shared.Rent(_indices.Length * 2);
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
      if (_static)
        Dispose();
      else
        Array.Clear(_indices, 0, _count);
    }

    private void Dispose()
    {
      _indices = null;
    }
  }
}