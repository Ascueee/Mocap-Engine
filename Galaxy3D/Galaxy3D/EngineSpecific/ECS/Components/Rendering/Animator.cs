namespace Galaxy3D.ECS.Components;

public struct Animator : IComponent
{
    private Dictionary<string, Animation> _animations;
    private Dictionary<int, string> _layers;
    private Dictionary<int, float> _layerTimes;
    private Dictionary<int, float> _layerSpeeds;

    public Animator()
    {
        _animations = new Dictionary<string, Animation>();
        _layers = new Dictionary<int, string>();
        _layerTimes = new Dictionary<int, float>();
        _layerSpeeds = new Dictionary<int, float>();
    }
    
    public void PlayOnLayer(int layer, string animationName, float speed = 1f)
    {
        _layers[layer] = animationName;
        _layerTimes[layer] = 0f;
        _layerSpeeds[layer] = speed;
    }

    public void StopLayer(int layer)
    {
        _layers.Remove(layer);
        _layerTimes.Remove(layer);
        _layerSpeeds.Remove(layer);
    }

    public int componentID { get; set; }
    public bool isActive { get; set; }
    public Dictionary<string, Animation> animations => _animations;
    public Dictionary<int, string> layers => _layers;
    public Dictionary<int, float> layerTimes => _layerTimes;
    public Dictionary<int, float> layerSpeeds => _layerSpeeds;
}