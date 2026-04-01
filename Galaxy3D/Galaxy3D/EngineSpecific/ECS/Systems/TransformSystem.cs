using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Physics;
using OpenTK.Mathematics;
namespace Galaxy3D.ECS.Systems;
public class TransformSystem : ISystem
{
    private Entity[] _transformEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;


    public TransformSystem(int MAX_ENTITIES)
    {
        _transformEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }
    
    public void UpdateSystem()
    {
       
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            Entity e = _transformEntities[i];
            Transform t = e.GetComponent<Transform>();

            if (t.isDirty)
            {
                SetDirty(e);
                
                UpdateWorldMatrix(e);
            }
            
        }
    }

    void SetDirty(Entity e)
    {
        foreach (Entity child in e.children)
        {
            Transform childTransform = child.GetComponent<Transform>();
            childTransform.isDirty = true;
            child.SetComponent(childTransform);
            SetDirty(child);
        }
    }
    
    private void UpdateWorldMatrix(Entity e)
    {
        Transform t = e.GetComponent<Transform>();

        if (e.HasComponent<AABB>())
        {
            AABB aabb = e.GetComponent<AABB>();

            if (e.parent is not null)
            {
                Transform parentTransform = e.parent.GetComponent<Transform>();
                aabb.min = parentTransform.position + aabb.posOffset - aabb.minOffset;
                aabb.max = parentTransform.position + aabb.posOffset + aabb.maxOffset;
            }
            else
            {
                aabb.min = t.position - aabb.minOffset;
                aabb.max = t.position + aabb.maxOffset;
            }

            e.SetComponent(aabb);
        }

        
        t.rotationMatrix = Matrix4.CreateRotationY(t.rotation.Y) * 
                           Matrix4.CreateRotationX(t.rotation.X) * 
                           Matrix4.CreateRotationZ(t.rotation.Z);
        
        if (!t.animatorOwned)
            t.localMatrix = Matrix4.CreateScale(t.scale) * t.rotationMatrix * Matrix4.CreateTranslation(t.position);

        if (e.parent is not null)
        {
            UpdateWorldMatrix(e.parent);
            Transform parentTransform = e.parent.GetComponent<Transform>();
            t.modelMatrix = t.localMatrix * parentTransform.modelMatrix;
        }
        else
        {
            t.modelMatrix = t.localMatrix;
        }

        t.isDirty = false;
        e.SetComponent(t);
    }
        

    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _transformEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _transformEntities.CopyTo(updatedArray, 0);
            _transformEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _transformEntities[id] = e;
        _currentSystemAmount++;
    }
}