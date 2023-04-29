using New.Shared;
using OpenTK.Graphics.OpenGL4;
using StbTrueTypeSharp;

namespace New.Engine;

public static class Font
{
  private const float _w = 1.0f / 2048f;
  private const float _h = _w;
  private static readonly float _ascent;
  private static readonly StbTrueType.stbtt_packedchar[] _chars;

  public static int Height;
  public static Image2d Image2d;

  static unsafe Font()
  {
    byte[] buffer = File.ReadAllBytes("Resource/Font/Dank Mono Italic.otf");
    int height = 20;
    Font.Height = height;

    StbTrueType.stbtt_fontinfo fontInfo = StbTrueType.CreateFont(buffer, 0);

    _chars = new StbTrueType.stbtt_packedchar[256];
    StbTrueType.stbtt_pack_context packContext = new();

    byte[] bitmap = new byte[2048 * 2048];
    fixed (byte* dat = bitmap)
    {
      StbTrueType.stbtt_PackBegin(packContext, dat, 2048, 2048, 0, 1, null);
    }

    StbTrueType.stbtt_PackSetOversampling(packContext, 8, 8);
    fixed (byte* dat = buffer)
    {
      fixed (StbTrueType.stbtt_packedchar* c = _chars)
      {
        StbTrueType.stbtt_PackFontRange(packContext, dat, 0, height, 32, 256, c);
      }
    }

    StbTrueType.stbtt_PackEnd(packContext);

    int asc;
    StbTrueType.stbtt_GetFontVMetrics(fontInfo, &asc, null, null);
    _ascent = asc * StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, height);

    Image2d = Image2d.from_buffer(bitmap, 2048, 2048, PixelFormat.Red, PixelInternalFormat.R8,
      TextureMinFilter.Nearest, TextureMagFilter.Nearest);
  }

  public static void Bind()
  {
    Image2d.Bind(TextureUnit.Texture0);
  }

  public static void Unbind()
  {
    Image2d.Unbind();
  }

  public static void Draw(ReadOnlySpan<char> text, float x, float y, uint color, bool shadow, float scale = 1.0f, Mesh<PCT> mesh1 = null)
  {
    Mesh<PCT> mesh = mesh1 ?? RenderSystem.FONT;
    int length = text.Length;
    float drawX = x;
    float drawY = y + _ascent * scale;
    float a = ((color >> 24) & 0xFF) / 255.0f;
    float r = ((color >> 16) & 0xFF) / 255.0f;
    float g = ((color >> 8) & 0xFF) / 255.0f;
    float b = (color & 0xFF) / 255.0f;
    for (int i = 0; i < length; i++)
    {
      char charCode = text[i];
      char previous = i > 0 ? text[i - 1] : ' ';
      if (previous == '\u00a7') continue;

      if (charCode == '\u00a7' && i < length - 1)
      {
        char next = char.ToLowerInvariant(text[i + 1]);
        if (FontFormat.VALUES.TryGetValue(next, out FontFormat fmt))
        {
          uint newColor = fmt.Color;
          r = ((newColor >> 16) & 0xFF) / 255.0f;
          g = ((newColor >> 8) & 0xFF) / 255.0f;
          b = (newColor & 0xFF) / 255.0f;
        }

        continue;
      }

      if (charCode < 32 || charCode > 32 + 256) charCode = ' ';

      StbTrueType.stbtt_packedchar c = _chars[charCode - 32];

      float dxs = drawX + c.xoff * scale;
      float dys = drawY + c.yoff * scale;
      float dx1S = drawX + c.xoff2 * scale;
      float dy1S = drawY + c.yoff2 * scale;
      float r1 = r * 0.125f;
      float g1 = g * 0.125f;
      float b1 = b * 0.125f;

      if (shadow)
      {
        mesh.Quad(
          mesh.Put(new(dxs + 1, dys - 1, 1, r1, g1, b1, a, c.x0 * _w, c.y0 * _h)),
          mesh.Put(new(dxs + 1, dy1S - 1, 1, r1, g1, b1, a, c.x0 * _w, c.y1 * _h)),
          mesh.Put(new(dx1S + 1, dy1S - 1, 1, r1, g1, b1, a, c.x1 * _w, c.y1 * _h)),
          mesh.Put(new(dx1S + 1, dys - 1, 1, r1, g1, b1, a, c.x1 * _w, c.y0 * _h))
        );
      }

      mesh.Quad(
        mesh.Put(new(dxs, dys, 0, r, g, b, a, c.x0 * _w, c.y0 * _h)),
        mesh.Put(new(dxs, dy1S, 0, r, g, b, a, c.x0 * _w, c.y1 * _h)),
        mesh.Put(new(dx1S, dy1S, 0, r, g, b, a, c.x1 * _w, c.y1 * _h)),
        mesh.Put(new(dx1S, dys, 0, r, g, b, a, c.x1 * _w, c.y0 * _h))
      );

      drawX += c.xadvance * scale;
      drawX -= 0.4f * scale;
    }
  }

  public static float get_width(string text, float scale = 1.0f)
  {
    int length = text.Length;
    float width = 0;
    for (int i = 0; i < length; i++)
    {
      char charCode = text[i];
      char previous = i > 0 ? text[i - 1] : ' ';
      if (previous == '\u00a7') continue;

      if (charCode < 32 || charCode > 32 + 256) charCode = ' ';

      StbTrueType.stbtt_packedchar c = _chars[charCode - 32];

      width += c.xadvance * scale;
      width -= 0.4f * scale;
    }

    width += 0.4f * scale;

    return width;
  }

  public static float get_height(float scale = 1.0f)
  {
    return _ascent * scale;
  }
}