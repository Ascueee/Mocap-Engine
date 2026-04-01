using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components.Physics;

public struct PhysicsBody : IComponent
{
    
    int _componentID;
    bool _isActive;
    Vector3 _veclovity;
    bool _isStatic;

    public PhysicsBody()
    {
        _veclovity = Vector3.Zero;
        _isStatic = true;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public bool isStatic {get => _isStatic; set => _isStatic = value; }
    public Vector3 velocity {get => _veclovity; set => _veclovity = value; }
}