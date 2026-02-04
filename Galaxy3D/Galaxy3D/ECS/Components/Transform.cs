using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct Transform : IComponent
{
    private int _componentID;
    private bool _isActive;
    Vector3 _position;
    Vector3 _rotation;
    Vector3 _scale;
    
    Matrix4 _modelMatrix;
    Matrix4 _rotationMatrix;
    
    public Transform()
    {
        _position = Vector3.Zero;
        _rotation = Vector3.Zero;
        _scale = Vector3.One;
        
        UpdareTransform();
    }
    
    public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        _position = position;
        _rotation = rotation;
        _scale = scale;
        
        UpdareTransform();
    }

    void UpdareTransform()
    {
        _rotationMatrix = Matrix4.CreateRotationX(_rotation.X) * 
                          Matrix4.CreateRotationY(_rotation.Y) * 
                          Matrix4.CreateRotationZ(_rotation.Z);
        
        _modelMatrix = Matrix4.CreateTranslation(_position) *
                       _rotationMatrix * 
                       Matrix4.CreateScale(_scale);
    }
    
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public Vector3 position {get => _position; set => _position = value; }
    public Vector3 rotation {get => _rotation; set => _rotation = value; }
    public Vector3 scale {get => _scale; set => _scale = value; }
    
    public Matrix4 rotationMatrix {get => _rotationMatrix; set => _rotationMatrix = value; }
    public Matrix4 modelMatrix {get => _modelMatrix; set => _modelMatrix = value; }
}