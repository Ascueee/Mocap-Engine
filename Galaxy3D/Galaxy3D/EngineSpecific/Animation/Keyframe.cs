using OpenTK.Mathematics;

namespace Galaxy3D.EngineSpecific;

public struct Keyframe
{
    private float _time;
    private Vector3 _rotation;

    public Keyframe(float time, Vector3 rotation)
    {
        _time = time;
        _rotation = rotation;
    }
    
    public float time => _time;
    public Vector3 rotation => _rotation;
}