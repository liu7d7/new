using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace New.Engine;

public sealed class Image2d
{
  private readonly int _handle;

  public readonly float Height;
  public readonly float Width;

  private Image2d(int glHandle, int width, int height)
  {
    _handle = glHandle;
    Width = width;
    Height = height;
  }

  public static Image2d from_file(string path, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
  {
    int handle = GL.GenTexture();

    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.Texture2D, handle);

    StbImage.stbi_set_flip_vertically_on_load(1);
    using Stream stream = File.OpenRead(path);
    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

    GL.TexImage2D(TextureTarget.Texture2D,
      0,
      PixelInternalFormat.Rgba,
      image.Width,
      image.Height,
      0,
      PixelFormat.Rgba,
      PixelType.UnsignedByte,
      image.Data);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

    return new Image2d(handle, image.Width, image.Height);
  }

  public static Image2d from_buffer(byte[] buffer, int width, int height, PixelFormat format,
    PixelInternalFormat internalFormat, TextureMinFilter minFilter = TextureMinFilter.LinearMipmapLinear,
    TextureMagFilter magFilter = TextureMagFilter.Linear, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
  {
    int handle = GL.GenTexture();

    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.Texture2D, handle);

    GL.TexImage2D(TextureTarget.Texture2D,
      0,
      internalFormat,
      width,
      height,
      0,
      format,
      PixelType.UnsignedByte,
      buffer);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

    return new Image2d(handle, width, height);
  }

  public void Bind(TextureUnit unit)
  {
    GL.ActiveTexture(unit);
    GL.BindTexture(TextureTarget.Texture2D, _handle);
  }

  public static void Bind(int id, TextureUnit unit)
  {
    GL.ActiveTexture(unit);
    GL.BindTexture(TextureTarget.Texture2D, id);
  }

  public static void Unbind()
  {
    GL.BindTexture(TextureTarget.Texture2D, 0);
  }
}