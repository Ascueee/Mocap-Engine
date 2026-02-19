using Galaxy3D.ECS.Components;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Systems;

/// <summary>
/// Need to itterate through each entity with a transform
/// Also need to update the component if the 
/// </summary>
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
    
public void UseSystem()
{
   
    for (int i = 0; i < _currentSystemAmount; i++)
    {
        Entity e = _transformEntities[i];
        Transform t = e.GetComponent<Transform>();

        if (t.isDirty)
        {
            //Then we need to update all the childran to dirty as well
            SetDirty(e);
            
            UpdateWorldMatrix(e);
        }
        //Console.WriteLine($"The entity is {e.entityName} and is entity dirty: {t.isDirty}");
    }
}

void SetDirty(Entity e)
{
    foreach (Entity child in e.children)
    {
        Transform childTransform = child.GetComponent<Transform>();
        Console.WriteLine("Child name is " + child.entityName);
        childTransform.isDirty = true;
        child.SetComponent(childTransform);
        SetDirty(child);
    }
}

//NOTES TO SELF:
//When a part of the transform is updated like the position, rotation, scale 
//The 


private void UpdateWorldMatrix(Entity e)
{
    Transform t = e.GetComponent<Transform>();
    
    t.rotationMatrix = Matrix4.CreateRotationX(t.rotation.X) *
                       Matrix4.CreateRotationY(t.rotation.Y) * 
                       Matrix4.CreateRotationZ(t.rotation.Z);

    Matrix4 localMatrix = Matrix4.CreateScale(t.scale) * t.rotationMatrix * Matrix4.CreateTranslation(t.position);

    if (e.parent is not null)
    {
        UpdateWorldMatrix(e.parent);
        //Console.WriteLine("Parent name is " + e.parent.entityName);
        Transform parentTransform = e.parent.GetComponent<Transform>();
        t.modelMatrix = localMatrix * parentTransform.modelMatrix;
    }
    else
    {
        t.modelMatrix = localMatrix;
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