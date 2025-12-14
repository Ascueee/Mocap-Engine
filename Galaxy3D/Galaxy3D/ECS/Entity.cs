using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;

namespace Galaxy3D;

public class Entity
{
    //hard coded amount of components per entity
    static long MAX_COMPONENTS = 100000000;
    static IdGenerator componentIdGenerator = new IdGenerator(MAX_COMPONENTS);
    
    private int _id;
    private string _entityName;
    static IComponent[] _components = new IComponent[MAX_COMPONENTS];
    
    
    Entity _parent;
    List<Entity> _children = new List<Entity>();
    
    public Entity(int id, string entityName)
    {
        _id = id;
        _entityName = entityName;
    }

    public void AddComponent(IComponent component)
    {
        component.componentID = componentIdGenerator.CreateEntityID();
        _components[component.componentID] = component;
    }
    
    public T GetComponent<T>() where T : IComponent
    {
        for(int i = 0; i < _components.Length; i++)
        {
            if (_components[i] is not null)
            {
                if (_components[i] is T)
                {
                    var temp = (T)_components[i];
                    if (temp is T)
                        return temp;
                }
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Red;
        throw new Exception($"Component of type {typeof(T).Name} not found");
    }
    
    public void SetComponent<T>(T updatedComponent) where T : IComponent
    {
        for(int i = 0; i < _components.Length; i++)
        {
            if (_components[i] is not null)
            {
                if (_components[i] is T)
                {
                    _components[i] = updatedComponent;
                    break;
                }
            }
        }
    }
    public bool HasComponent<T>() where T : IComponent
    {
        for(int i = 0; i < _components.Length; i++)
        {
            if (_components[i] is not null)
            {
                IComponent temp = _components[i];
                if (temp is T)
                    return true;
            }
        }

        return false;
    }

    public void PrintComponents()
    {
        Console.WriteLine($"Components in {entityName}:");
        for (int i = 0; i < _components.Length; i++)
        {
            if (_components[i] is null)
                break;
            Console.WriteLine($"\tComponent | {_components[i].GetType().Name}");
        }
    }
    
    public int id => _id;
    public string entityName => _entityName;

}