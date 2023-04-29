using System.Buffers;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace New.Engine;

public sealed class Vbo<T> where T : struct, IPos3d
{
  public T[] Vertices;

  private readonly int _handle;
  private readonly bool _static;
  private int _count;
  private static readonly int _size = Marshal.SizeOf<T>();

  public Vbo(int initialCapacity, bool @static)
  {
    _handle = GL.GenBuffer();
    Vertices = ArrayPool<T>.Shared.Rent(initialCapacity);
    _static = @static;
  }

  public void Bind()
  {
    GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);
  }

  public static void Unbind()
  {
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
  }

  public void Put(T element)
  {
    if (_count + 1 > Vertices.Length)
    {
      ArrayPool<T>.Shared.Return(Vertices);
      T[] prev = Vertices;
      Vertices = ArrayPool<T>.Shared.Rent(Vertices.Length * 2);
      Array.Copy(prev, Vertices, _count);
    }

    Vertices[_count] = element;
    _count++;
  }

  public void Upload(bool unbindAfter = true)
  {
    Bind();
    GL.BufferData(BufferTarget.ArrayBuffer, _count * _size, Vertices, _static ? BufferUsageHint.StaticDraw : BufferUsageHint.DynamicDraw);
    if (unbindAfter) Unbind();
  }

  public void Clear()
  {
    _count = 0;
    Array.Clear(Vertices, 0, _count);
  }
}