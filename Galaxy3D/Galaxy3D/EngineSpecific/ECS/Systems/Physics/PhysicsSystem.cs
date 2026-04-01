using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Physics;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Systems;

public class PhysicsSystem : ISystem
{
    private Entity[] _physicsEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;

    private float _gravity;

    public PhysicsSystem(int MAX_ENTITIES, float gravity)
    {
        _physicsEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
        _gravity = gravity;
    }
    
    public void UpdateSystem(float deltaTime)
    {
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            PhysicsBody entityPhysicsBody = _physicsEntities[i].GetComponent<PhysicsBody>();
            Transform entityTransform = _physicsEntities[i].GetComponent<Transform>();
            
            if (entityPhysicsBody.isStatic == false)
            {
                //Adds gravity to the physics body
                entityPhysicsBody.velocity += new Vector3(0, _gravity, 0) * deltaTime;
                _physicsEntities[i].SetComponent(entityPhysicsBody);
            }
        }
    }


    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _physicsEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _physicsEntities.CopyTo(updatedArray, 0);
            _physicsEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _physicsEntities[id] = e;
        _currentSystemAmount++;
    }
}