using Galaxy3D.Assets;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct Material : IComponent
{
    private int _componentID;
    private bool _isActive;
    private int _atlasId;//Determines the texture position in a atlas
    private int _previousID;
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
        _shine = shine;
    }
    public Material(Shader shader, TextureAtlas texture, Vector4 materialColor, float specularStrength, float shine, int id)
    {
        _shader = shader;
        _texture = texture;
        _materialColor = new Vector4(1f);
        _specularStrength = new Vector3(specularStrength);
        _shine = shine;
        _atlasId = id;
        _previousID = id;
        
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public int atlasID {get => _atlasId; set => _atlasId = value; }
    public int previousID { get => _previousID; set => _previousID = value; }
    public Shader shader { get => _shader; }
    public Texture texture { get => _texture; }
    public Vector4 materialColor { get => _materialColor; set => _materialColor = value; } 
    public Vector3 specularStrenght{get => _specularStrength; set => _specularStrength = value; }
    public float shine { get => _shine; set => _shine = value; }
    
}