using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Rasterization;

public class Texture
{
    // data members
    public int Id;

    // constructor
    public Texture(string filename)
    {
        if (string.IsNullOrEmpty(filename)) throw new ArgumentException(filename);
        Id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, Id);
        // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
        // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
        // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        var bmp = Image.Load<Bgra32>(filename);
        var width = bmp.Width;
        var height = bmp.Height;
        var pixels = new int[width * height];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
            pixels[y * width + x] = (int)bmp[x, y].Bgra;
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra,
            PixelType.UnsignedByte, pixels);
    }
}