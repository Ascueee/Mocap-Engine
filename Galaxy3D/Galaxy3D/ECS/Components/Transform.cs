namespace Galaxy3D.ECS.Components;

public struct Transform : IComponent
{
    private int _componentID;
    
    public int componentID { get; set; }
}