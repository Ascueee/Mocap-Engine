using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Components;

public struct Camera : IComponent
{
    int _componentID;
    bool _isActive;
    
    private float _pitch = -90;
    private float _yaw;
    private float _fov;
    private float _aspectRatio;
    private bool _unfocused = false;
    Matrix4 _projectionMatrix;
    Matrix4 _viewMatrix;
    
    Vector3 _front = new Vector3(0.0f, 0.0f, -1.0f);
    Vector3 _up = new Vector3(0.0f, 1.0f,  0.0f);
    Vector3 _right = new Vector3(1.0f, 0.0f,  0.0f);

    public Camera(float aspectRatio, float fov)
    {
        _aspectRatio = aspectRatio;
        _fov = MathHelper.DegreesToRadians(fov);
    }

    public int componentID { get; set; }
    public bool isActive { get; set; }
    public float fov { get => _fov; set => _fov = value; }
    public float aspectRatio { get => _aspectRatio; set => _aspectRatio = value; }
    public float pitch { get => _pitch; set => _pitch = value; }
    public float yaw { get => _yaw; set => _yaw = value; }
    public bool unfocused { get => _unfocused; set => _unfocused = value; }
    public Matrix4 projectionMatrix { get => _projectionMatrix; set => _projectionMatrix = value; }
    public Matrix4 viewMatrix { get => _viewMatrix; set => _viewMatrix = value; }
    public Vector3 front { get => _front; set => _front = value; }
    public Vector3 up { get => _up; set => _up = value; }
    public Vector3 right { get => _right; set => _right = value; }
    
    
}