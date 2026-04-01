using OpenTK.Mathematics;
namespace Galaxy3D.ECS.Components.Physics;
public struct AABB : IComponent
{
    int _componentID;
    bool _isActive;
    bool _isColliding;
    private bool _isTrigger;
    Vector3 _posOffset;
    Vector3 _minOffset;
    Vector3 _maxOffset; 
    Vector3 _min;
    Vector3 _max; 

    public AABB()
    {
        _minOffset = Vector3.One;
        _maxOffset = Vector3.One;
        _posOffset = Vector3.Zero;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public bool isColliding {get => _isColliding; set => _isColliding = value; }
    public bool isTrigger { get => _isTrigger; set => _isTrigger = value; }
    public Vector3 posOffset { get => _posOffset; set => _posOffset = value; }
    public Vector3 minOffset { get => _minOffset; set => _minOffset = value; }
    public Vector3 maxOffset { get => _maxOffset; set => _maxOffset = value; }
    public Vector3 min { get => _min; set => _min = value; }
    public Vector3 max { get => _max; set => _max = value; }

    
}