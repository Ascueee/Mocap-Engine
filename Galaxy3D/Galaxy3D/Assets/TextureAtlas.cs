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
    //Needs to itterate past the first three floats of the mesh data because thats vertex data
    //Then needs to take the UVS which are the 4th and 5th element of the vertex
    //BUT need to add a way to make sure that I dont change Normals or other vertex data later so itterate after that
    public float[] UpdateMeshUVs(int textureID, float[] modelMesh)
    {
        //RN ONLY WORKS FOR CUBE NEED TO CHANGE TO ALLOW FOR THE MESH RENDER TO HOLD VERTEX AND MESH DATA SEPERATLY
        //when I get the uvs now update them in the array
        //Uvs need to represent the texture in texture coordinate space and normalized
        //need to retrieve the first two normalized texture coordinates
        //then need for the second two vertices need to  add with the _textureSize to on the X to get the x value of
        //the second set of vertices and the Ys are either 0 or the _textureSize in my system.
        //Works for square uvs needs to set specifically for each shape
            
        //This would be the first value for the normalized UVS
        var u = (textureID * _textureSize) / _textureResolution.X;
        var v = 0 / _textureResolution.Y;
            
        var u1 = (u + _textureSize) / _textureResolution.X;
        var v1 = (_textureSize) / _textureResolution.Y;
        return new float[]
        {
            //Vertex data layout: First three vertex data, then the next two are UVs
            // Back face
            -0.5f, -0.5f, -0.5f, u, v,
            0.5f, -0.5f, -0.5f, u1, v,
            0.5f,  0.5f, -0.5f, u1, v1,
            0.5f,  0.5f, -0.5f, u1, v1,
            -0.5f,  0.5f, -0.5f, u, v1,
            -0.5f, -0.5f, -0.5f, u, v,

            // Front face
            -0.5f, -0.5f,  0.5f, u, v,
            0.5f, -0.5f,  0.5f, u1, v,
            0.5f,  0.5f,  0.5f, u1, v1,
            0.5f,  0.5f,  0.5f, u1, v1,
            -0.5f,  0.5f,  0.5f, u, v1,
            -0.5f, -0.5f,  0.5f, u, v,

            // Left face
            -0.5f,  0.5f,  0.5f, u1, v1,
            -0.5f,  0.5f, -0.5f, u, v1,
            -0.5f, -0.5f, -0.5f, u, v,
            -0.5f, -0.5f, -0.5f, u, v,
            -0.5f, -0.5f,  0.5f, u1, v,
            -0.5f,  0.5f,  0.5f, u1, v1,

            // Right face
            0.5f,  0.5f,  0.5f, u1, v1,
            0.5f,  0.5f, -0.5f, u, v1,
            0.5f, -0.5f, -0.5f, u, v,
            0.5f, -0.5f, -0.5f, u, v,
            0.5f, -0.5f,  0.5f, u1, v,
            0.5f,  0.5f,  0.5f, u1, v1,

            // Bottom face
            -0.5f, -0.5f, -0.5f, u, v1,
            0.5f, -0.5f, -0.5f, u1, v1,
            0.5f, -0.5f,  0.5f, u1, v,
            0.5f, -0.5f,  0.5f, u1, v,
            -0.5f, -0.5f,  0.5f, u, v,
            -0.5f, -0.5f, -0.5f, u, v1,

            // Top face
            -0.5f,  0.5f, -0.5f, u, v1,
            0.5f,  0.5f, -0.5f, u1, v1,
            0.5f,  0.5f,  0.5f, u1, v,
            0.5f,  0.5f,  0.5f, u1, v,
            -0.5f,  0.5f,  0.5f, u, v,
            -0.5f,  0.5f, -0.5f, u, v1,
        };
    }
    
}