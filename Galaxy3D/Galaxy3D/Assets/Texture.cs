using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;
namespace Galaxy3D.Assets;

public enum TextureType
{
    Base,
    Skybox
}
public class Texture
{
    private string[] _texturePath = new string[6];
    private int _handle;
    public Vector2 _textureResolution;
    TextureType _textureType;

    public Texture(string texturePath)
    {
        _texturePath[0] = texturePath;
        _textureType = TextureType.Base;
    }

    public Texture(string[] texturePaths)
    {
        _texturePath = texturePaths;
        _textureType = TextureType.Skybox;
    }

    public virtual void Load()
    {
        _handle = GL.GenTexture();

        if (_textureType == TextureType.Base)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        
            StbImage.stbi_set_flip_vertically_on_load(1);
            using (Stream stream = File.OpenRead(_texturePath[0]))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                _textureResolution = new Vector2(image.Width, image.Height);
            
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            }
        
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        else if (_textureType == TextureType.Skybox)
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, _handle);
            StbImage.stbi_set_flip_vertically_on_load(0);
            
            for (int i = 0; i < 6; i++)
            {
                using var stream = File.OpenRead(_texturePath[i]);
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);
                _textureResolution = new Vector2(image.Width, image.Height);

                GL.TexImage2D(
                    TextureTarget.TextureCubeMapPositiveX + i, 
                    0,
                    PixelInternalFormat.Rgb,
                    image.Width, image.Height, 0,
                    PixelFormat.Rgb,
                    PixelType.UnsignedByte,
                    image.Data
                );
            }
            
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        }

    }

    public virtual void Use(TextureUnit textureUnit)
    {
        GL.ActiveTexture(textureUnit);

        if (_textureType == TextureType.Skybox)
            GL.BindTexture(TextureTarget.TextureCubeMap, _handle);
        else
            GL.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
    public int handle { get => _handle; }
}