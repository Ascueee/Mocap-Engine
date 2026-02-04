using Galaxy3D.ECS.Components;

namespace Galaxy3D.ECS;

public class Entity
{
    //hard coded amount of components per entity
    int _maxEntities = 100;
    IdGenerator componentIdGenerator = new IdGenerator(100);
    private int entityComponentAmount = 0;
    
    private int _id;
    private string _entityName;
    IComponent[] _components = new IComponent[100];
    
    //this is the child parent hierachy for the entities
    Entity _parent;
    Dictionary<string, Entity> _children = new Dictionary<string, Entity>();
    
    public Entity(int id, string entityName)
    {
        _id = id;
        _entityName = entityName;
    }

    public void AddComponent(IComponent component)
    {
        if (entityComponentAmount == _maxEntities)
        {
            _maxEntities += 100;
            componentIdGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new IComponent[_maxEntities];
            _components.CopyTo(updatedArray, 0);
            _components = updatedArray;
        }
        
        component.componentID = componentIdGenerator.CreateEntityID();
        _components[component.componentID] = component;
        entityComponentAmount++;
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

    public void SetChild(Entity child)
    {
        child.parent = this;
        children.Add(child._entityName, child);
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
    public Entity parent { get => _parent; set => _parent = value; }
    public Dictionary<string, Entity> children => _children;

}