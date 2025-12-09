using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Galaxy3D.Assets;

/// <summary>
/// Loads and image file that should multiple textures
/// Takes in the set texture size(textures need to be sqaure)
/// Then needs a method to that takes a textureID(the location in the texture)
/// Then use that textureID to retrive texture coordinates(uvs) and set it in the mesh coordinates.
/// </summary>
public class TextureAtlas : Texture
{
    private string _texturePath;
    private int _handle;
    private int _textureSize; //Needs to be a consistant size ex(64x64) going to divide the texture size by the resolution to retrieve the right texture
    Vector2 _textureResolution;
    
    public TextureAtlas(string texturePath, int textureSize ,Vector2 textureResolution) : base(texturePath)
    {
        _texturePath = texturePath;
        _textureSize = textureSize;
        _textureResolution = textureResolution;
    }
    
    public override void Load()
    {
        base.Load();
    }

    public override void Use(TextureUnit textureUnit)
    {
        base.Use(textureUnit);
    }

    //Updates a meshes data with new texture data 
    //It First: Gets the texture height and width
    //Then it uses the texture id which represents the textures place in the coordinate
    //then itterates through the modelUVs and updates them to match with the texture
    public float[] UpdateMeshUVs(int textureID, float[] modelUVs)
    {
        float[] updatedUVs = new float[modelUVs.Length];

        
        float tileW = _textureSize / _textureResolution.X;
        float tileH = _textureSize / _textureResolution.Y;
        
        float uOffset = (textureID * _textureSize) / _textureResolution.X;
        float vOffset = (_textureSize) / _textureResolution.Y;

        for (int i = 0; i < modelUVs.Length; i += 2)
        {
            float modelU = modelUVs[i];
            float modelV = modelUVs[i + 1];
            
            updatedUVs[i]     = uOffset + modelU * tileW;
            updatedUVs[i + 1] = vOffset + modelV * tileH;
        }

        return updatedUVs;
    }
    
}