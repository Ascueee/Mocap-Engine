namespace Galaxy3D.ECS.Systems.ComponentSystems;

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

    public void AddEntityToSystemw(Entity e)
    {
    }
}