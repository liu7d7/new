using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace New.Engine
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct P : IPos3d
  {
    public Vector3 Pos { get; set; }

    public P(float x, float y, float z)
    {
      Pos = new Vector3(x, y, z);
    }

    public P(float x, float y)
    {
      Pos = new Vector3(x, y, 0);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct PC : IPos3d
  {
    public Vector3 Pos { get; set; }
    public Vector4 Color { get; set; }

    public PC(float x, float y, float z, float r, float g, float b, float a)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, a);
    }

    public PC(float x, float y, float z, float r, float g, float b)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, 1);
    }

    public PC(float x, float y, float z, uint color)
    {
      Pos = new Vector3(x, y, z);
      Color = color.ToVector4();
    }

    public PC(Vector3 pos, Vector4 color)
    {
      Pos = pos;
      Color = color;
    }

    public PC(Vector3 pos, uint color)
    {
      Pos = pos;
      Color = color.ToVector4();
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct PWC : IPos3d
  {
    public Vector3 Pos { get; set; }
    public float Width { get; set; }
    public Vector4 Color { get; set; }

    public PWC(float x, float y, float z, float w, float r, float g, float b, float a)
    {
      Pos = new Vector3(x, y, z);
      Width = w;
      Color = new Vector4(r, g, b, a);
    }

    public PWC(float x, float y, float z, float w, float r, float g, float b)
    {
      Pos = new Vector3(x, y, z);
      Width = w;
      Color = new Vector4(r, g, b, 1);
    }

    public PWC(float x, float y, float z, float w, uint color)
    {
      Pos = new Vector3(x, y, z);
      Width = w;
      Color = color.ToVector4();
    }

    public PWC(Vector3 pos, float width, Vector4 color)
    {
      Pos = pos;
      Width = width;
      Color = color;
    }

    public PWC(Vector3 pos, float width, Color4 color)
    {
      Pos = pos;
      Width = width;
      Color = color.ToVector4();
    }

    public PWC(Vector3 pos, float width, uint color)
    {
      Pos = pos;
      Width = width;
      Color = color.ToVector4();
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct PCT : IPos3d
  {
    public Vector3 Pos { get; set; }
    public Vector4 Color { get; set; }
    public Vector2 TexCoord { get; set; }

    public PCT(float x, float y, float z, float r, float g, float b, float a, float u, float v)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, a);
      TexCoord = new Vector2(u, v);
    }

    public PCT(float x, float y, float z, float r, float g, float b, float u, float v)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, 1);
      TexCoord = new Vector2(u, v);
    }

    public PCT(float x, float y, float z, uint color, float u, float v)
    {
      Pos = new Vector3(x, y, z);
      Color = color.ToVector4();
      TexCoord = new Vector2(u, v);
    }

    public PCT(Vector3 pos, Vector4 color, Vector2 texCoord)
    {
      Pos = pos;
      Color = color;
      TexCoord = texCoord;
    }

    public PCT(Vector3 pos, uint color, Vector2 texCoord)
    {
      Pos = pos;
      Color = color.ToVector4();
      TexCoord = texCoord;
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct PCTT : IPos3d
  {
    public Vector3 Pos { get; set; }
    public Vector4 Color { get; set; }
    public Vector2 TexCoord { get; set; }
    public float TexIndex { get; set; }

    public PCTT(float x, float y, float z, float r, float g, float b, float a, float u, float v, float i)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, a);
      TexCoord = new Vector2(u, v);
      TexIndex = i;
    }

    public PCTT(float x, float y, float z, float r, float g, float b, float u, float v, float i)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, 1);
      TexCoord = new Vector2(u, v);
      TexIndex = i;
    }

    public PCTT(float x, float y, float z, uint color, float u, float v, float i)
    {
      Pos = new Vector3(x, y, z);
      Color = color.ToVector4();
      TexCoord = new Vector2(u, v);
      TexIndex = i;
    }

    public PCTT(Vector3 pos, Vector4 color, Vector2 texCoord, float texIndex)
    {
      Pos = pos;
      Color = color;
      TexCoord = texCoord;
      TexIndex = texIndex;
    }

    public PCTT(Vector3 pos, uint color, Vector2 texCoord, float texIndex)
    {
      Pos = pos;
      Color = color.ToVector4();
      TexCoord = texCoord;
      TexIndex = texIndex;
    }

    public PCTT(Vector3 pos, uint color, Vector2 texCoord, int texIndex)
    {
      Pos = pos;
      Color = color.ToVector4();
      TexCoord = texCoord;
      TexIndex = texIndex;
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct PCN : IPos3d
  {
    public Vector3 Pos { get; set; }
    public Vector4 Color { get; set; }
    public Vector3 Normal { get; set; }

    public PCN(float x, float y, float z, float r, float g, float b, float a, float nx, float ny, float nz)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, a);
      Normal = new Vector3(nx, ny, nz);
    }

    public PCN(float x, float y, float z, float r, float g, float b, float nx, float ny, float nz)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, 1);
      Normal = new Vector3(nx, ny, nz);
    }

    public PCN(float x, float y, float z, uint color, float nx, float ny, float nz)
    {
      Pos = new Vector3(x, y, z);
      Color = color.ToVector4();
      Normal = new Vector3(nx, ny, nz);
    }

    public PCN(Vector3 pos, Vector4 color, Vector3 normal)
    {
      Pos = pos;
      Color = color;
      Normal = normal;
    }

    public PCN(Vector3 pos, uint color, Vector3 normal)
    {
      Pos = pos;
      Color = color.ToVector4();
      Normal = normal;
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct PCNTT : IPos3d
  {
    public Vector3 Pos { get; set; }
    public Vector4 Color { get; set; }
    public Vector3 Normal { get; set; }
    public Vector2 TexCoord { get; set; }
    public float TexIndex { get; set; }

    public PCNTT(float x, float y, float z, float r, float g, float b, float a, float nx, float ny, float nz, float u, float v, float i)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, a);
      Normal = new Vector3(nx, ny, nz);
      TexCoord = new Vector2(u, v);
      TexIndex = i;
    }

    public PCNTT(float x, float y, float z, float r, float g, float b, float nx, float ny, float nz, float u, float v, float i)
    {
      Pos = new Vector3(x, y, z);
      Color = new Vector4(r, g, b, 1);
      Normal = new Vector3(nx, ny, nz);
      TexCoord = new Vector2(u, v);
      TexIndex = i;
    }

    public PCNTT(float x, float y, float z, uint color, float nx, float ny, float nz, float u, float v, float i)
    {
      Pos = new Vector3(x, y, z);
      Color = color.ToVector4();
      Normal = new Vector3(nx, ny, nz);
      TexCoord = new Vector2(u, v);
      TexIndex = i;
    }

    public PCNTT(Vector3 pos, Vector4 color, Vector3 normal, Vector2 texCoord, float texIndex)
    {
      Pos = pos;
      Color = color;
      Normal = normal;
      TexCoord = texCoord;
      TexIndex = texIndex;
    }

    public PCNTT(Vector3 pos, uint color, Vector3 normal, Vector2 texCoord, float texIndex)
    {
      Pos = pos;
      Color = color.ToVector4();
      Normal = normal;
      TexCoord = texCoord;
      TexIndex = texIndex;
    }
  }

  public static class VertexTypes
  {
    public static Dictionary<Type, Vao.Attrib[]> Layouts = new Dictionary<Type, Vao.Attrib[]>
    {
      {
        typeof(P), new[]
          { Vao.Attrib.Float3 }
      },
      {
        typeof(PC), new[]
          { Vao.Attrib.Float3, Vao.Attrib.Float4 }
      },
      {
        typeof(PCN), new[]
          { Vao.Attrib.Float3, Vao.Attrib.Float4, Vao.Attrib.Float3 }
      },
      {
        typeof(PCNTT), new[]
          { Vao.Attrib.Float3, Vao.Attrib.Float4, Vao.Attrib.Float3, Vao.Attrib.Float2, Vao.Attrib.Float1 }
      },
      {
        typeof(PCT), new[]
          { Vao.Attrib.Float3, Vao.Attrib.Float4, Vao.Attrib.Float2 }
      },
      {
        typeof(PCTT), new[]
          { Vao.Attrib.Float3, Vao.Attrib.Float4, Vao.Attrib.Float2, Vao.Attrib.Float1 }
      },
      {
        typeof(PWC), new[]
          { Vao.Attrib.Float3, Vao.Attrib.Float1, Vao.Attrib.Float4 }
      },
    };
  }
}