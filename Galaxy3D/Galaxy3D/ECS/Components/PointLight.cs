using System.Drawing;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct PointLight : IComponent
{
    private Vector3 _lightColor;
    private Vector3 _ambient;
    private Vector3 _diffuse;
    private Vector3 _specular;
    
    float _constant;
    float _linear;
    float _quadratic;  

    public PointLight(Vector3 lightColor, Vector3 ambient, Vector3 diffuse, Vector3 specular,
        float constant, float linear, float quadratic)
    {
        _lightColor = lightColor;
        _ambient = ambient;
        _diffuse = diffuse;
        _specular = specular;
        
        _constant = constant;
        _linear = linear;
        _quadratic = quadratic;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    
    public Vector3 lightColor { get => _lightColor; set => _lightColor = value; }
    public Vector3 ambient { get => _ambient; set => _ambient = value; }
    public Vector3 diffuse { get => _diffuse; set => _diffuse = value; }
    public Vector3 specular { get => _specular; set => _specular = value; }
    public float constant { get => _constant; set => _constant = value; }
    public float linear { get => _linear; set => _linear = value; }
    public float quadratic { get => _quadratic; set => _quadratic = value; }
}