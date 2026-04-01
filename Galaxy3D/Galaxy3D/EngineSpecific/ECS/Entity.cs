using Galaxy3D.ECS.Components;

namespace Galaxy3D.ECS;

public class Entity
{
    //hard coded amount of components per entity
    int _maxComponents = 100;
    IdGenerator componentIdGenerator = new IdGenerator(100);
    private int entityComponentAmount = 0;
    
    private int _id;
    private string _entityName;
    IComponent[] _components = new IComponent[100];
    
    //this is the child parent hierachy for the entities
    Entity _parent;
    List<Entity> _children = new List<Entity>();
    private bool _alreadySorted = false;
    
    public Entity(int id, string entityName)
    {
        _id = id;
        _entityName = entityName;
    }

    public void AddComponent(IComponent component)
    {
        if (entityComponentAmount == _maxComponents)
        {
            _maxComponents += 100;
            componentIdGenerator.maxAmountOfIds = _maxComponents;
            var updatedArray = new IComponent[_maxComponents];
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

    public void SetChild(Entity e)
    {
        _children.Add(e);
        e.parent = this;
    }

    public Entity GetChild(string entityName)
    {
        foreach (var e in children)
        {
            if(e.entityName == entityName)
                return e;
        }
        
        return null;
    }
    
    public int id => _id;
    public string entityName {get => _entityName; set => _entityName = value; }
    public Entity parent { get => _parent; set => _parent = value; }
    public List<Entity> children { get => _children; set => _children = value; }
    public bool alreadySorted { get => _alreadySorted; set => _alreadySorted = value; }

}