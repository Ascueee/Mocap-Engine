namespace Galaxy3D.ECS.Systems;

public interface ISystem
{

    
    //Load all system logic
    public void LoadSystem()
    {
    }

    //Run system logic
    public void UpdateSystem()
    {
    }

    public void AddEntityToSystem(Entity e)
    {
        
    }
}