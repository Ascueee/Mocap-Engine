using OpenTK.Mathematics;
namespace Galaxy3D.ECS.Components;


public class Camera : IComponent
{
    public int componentID { get; set; }
    private Vector3 _front;
    private Vector3 _up;
    private Vector3 _right;
    private float _pitch;
    private float _yaw;
    private float _fov;

}