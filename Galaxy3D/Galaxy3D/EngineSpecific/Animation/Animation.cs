using Galaxy3D.EngineSpecific;

namespace Galaxy3D;

public struct Animation
{
    string _animationName;
    private float _animationDuration;
    private bool _isPlaying = false;
    bool _isLooping = false;
    //string will point to an entity name, then a list of keyframes which will animate that specific entity
    //allows for an animation to animate multiple entities in the model like humanoid body parts
    Dictionary<string, List<Keyframe>> _animatedEntities = new Dictionary<string, List<Keyframe>>();
    
    public Animation(string animationName, float animationDuration) {
        _animationName = animationName;
        _animationDuration = animationDuration;
    }
    
    public Dictionary<string, List<Keyframe>> AnimatedEntities => _animatedEntities;
    public bool isPlaying {get => _isPlaying; set => _isPlaying = value; }
    public bool isLooping { get => _isLooping; set => _isLooping = value; }
    public float AnimationDuration => _animationDuration;
}