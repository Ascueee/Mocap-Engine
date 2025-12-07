using Galaxy3D.ECS.Systems;
namespace Galaxy3D.ECS.Components;

public struct DebugComponent : IComponent
{
    int _componentID;
    public string msg;

    public DebugComponent(string msg)
    {
        this.msg = msg;
    }
    
    public int componentID { get { return _componentID; } set => _componentID = value; }

}