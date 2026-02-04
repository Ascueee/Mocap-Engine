using Galaxy3D.Assets;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct Material : IComponent
{
    private int _componentID;
    private bool _isActive;
    private Shader _shader;
    Texture _texture;
    
    
    //Shader attributes
    Vector4 _materialColor;
    Vector3 _specularStrength;
    float _shine;

    public Material(Shader shader, Texture texture, Vector4 materialColor, float specularStrength, float shine)
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
    public bool isActive { get; set; }
    public Shader shader { get => _shader; }
    public Texture texture { get => _texture; }
    public Vector4 materialColor { get => _materialColor; set => _materialColor = value; } 
    public Vector3 specularStrenght{get => _specularStrength; set => _specularStrength = value; }
    public float shine { get => _shine; set => _shine = value; }
    
}