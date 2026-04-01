using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct Transform : IComponent
{
    private int _componentID;
    private bool _isActive;
    private bool _isDirty;
    bool _preserveLocalMatrix; // NEW: localMatrix was set directly from Assimp, don't overwrite with TRS
    
    bool _animatorOwned;
    Vector3 _position;
    Vector3 _rotation;
    Vector3 _scale;
    
    Matrix4 _modelMatrix; //world space matrix
    Matrix4 _rotationMatrix;
    Matrix4 _localMatrix;
    
    
    public Transform()
    {
        _position = Vector3.Zero;
        _rotation = Vector3.Zero;
        _scale = Vector3.One;
        _isDirty = true;
    }
    
    public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        _position = position;
        _rotation = rotation;
        _scale = scale;
        _isDirty = true;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public bool isDirty { get => _isDirty; set => _isDirty = value; }
    public bool animatorOwned { get => _animatorOwned; set => _animatorOwned = value; }
    public bool preserveLocalMatrix { get => _preserveLocalMatrix; set => _preserveLocalMatrix = value; }
    public Vector3 position { get => _position; set { _position = value; _isDirty = true; } }

    public Vector3 rotation {get => _rotation; set { _rotation = value; _isDirty = true; _preserveLocalMatrix = false;} }
    public Vector3 scale {get => _scale; set { _scale = value; _isDirty = true; } }
    
    public Matrix4 rotationMatrix {get => _rotationMatrix; set => _rotationMatrix = value; }
    public Matrix4 modelMatrix {get => _modelMatrix; set => _modelMatrix = value; }
    public Matrix4 localMatrix {get => _localMatrix; set => _localMatrix = value; }
    
}