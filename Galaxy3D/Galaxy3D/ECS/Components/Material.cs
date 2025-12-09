
using Galaxy3D;
using Galaxy3D.Assets;
using OpenTK.Mathematics;
using Galaxy3D.ECS.Components;

namespace Soul.ECS.Components;

public struct Material : IComponent
{
    private int _componentID;
    private Shader _shader;
    Texture _texture;
    
    
    //Shader attributes
    Vector4 _materialColor;

    public Material(Shader shader, Texture texture, Vector4 materialColor)
    {
        _shader = shader;
        _texture = texture;
        _materialColor = materialColor;
    }
    
    public Material(Shader shader, Texture texture)
    {
        _shader = shader;
        _texture = texture;
        _materialColor = new Vector4(1f);
    }
    
    public Material(Shader shader, TextureAtlas texture)
    {
        _shader = shader;
        _texture = texture;
        _materialColor = new Vector4(1f);
    }
    
    public int componentID { get; set; }
    public Shader shader { get => _shader; }
    public Texture texture { get => _texture; }
    public Vector4 materialColor { get => _materialColor; set => _materialColor = value; } 
    
}