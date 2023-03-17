using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace New.Engine
{
  public class Ubo
  {
    private readonly int _handle;
    private readonly int[] _offsets;

    public Ubo(Shader shdr, string blkName, params string[] names)
    {
      _handle = GL.GenBuffer();
      int blockIndex = GL.GetUniformBlockIndex(shdr.Handle, blkName);
      GL.GetActiveUniformBlock(shdr.Handle, blockIndex, ActiveUniformBlockParameter.UniformBlockDataSize, out int blockSize);
      int[] indices = new int[names.Length];
      GL.GetUniformIndices(shdr.Handle, names.Length, names, indices);
      _offsets = new int[names.Length];
      GL.GetActiveUniforms(shdr.Handle, names.Length, indices, ActiveUniformParameter.UniformOffset, _offsets);
      Bind();
      GL.BufferData(BufferTarget.UniformBuffer, blockSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
      Unbind();
    }

    public void Bind()
    {
      GL.BindBuffer(BufferTarget.UniformBuffer, _handle);
    }

    public static void Unbind()
    {
      GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void PutAll<T>(ref T[] mats, int size, int offset) where T : struct
    {
      Bind();
      GL.BufferSubData(BufferTarget.UniformBuffer, _offsets[offset], size * Marshal.SizeOf<T>(), ref mats[0]);
    }

    public void BindTo(int bindingPoint)
    {
      GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint, _handle);
    }
  }
}