using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct DirectionalLight : IComponent
{
    private int _componentID;
    private bool _isActive;
    
    private Vector3 _lightColor;
    private Vector3 _ambient;
    private Vector3 _diffuse;
    private Vector3 _specular;

    public DirectionalLight(Vector3 lightColor, Vector3 ambient, Vector3 diffuse, Vector3 specular)
    {
        _lightColor = lightColor;
        _ambient = ambient;
        _diffuse = diffuse;
        _specular = specular;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public Vector3 lightColor { get => _lightColor; set => _lightColor = value; }
    public Vector3 ambient { get => _ambient; set => _ambient = value; }
    public Vector3 diffuse { get => _diffuse; set => _diffuse = value; }
    public Vector3 specular { get => _specular; set => _specular = value; }
}