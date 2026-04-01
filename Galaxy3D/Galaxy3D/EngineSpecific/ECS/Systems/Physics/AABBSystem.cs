using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Physics;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Systems;

public class AABBSystem : ISystem
{
    private Entity[] _AABBEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;

    public AABBSystem(int MAX_ENTITIES)
    {
        _AABBEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }
    
    public void LoadSystem()
    {
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            AABB entityBoundingBox = _AABBEntities[i].GetComponent<AABB>();
            Transform entityTransform = _AABBEntities[i].GetComponent<Transform>();
            entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
            entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;
            _AABBEntities[i].SetComponent(entityBoundingBox);
        }
    }

    public void UpdateSystem(float deltaTime)
    {
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            AABB entityBoundingBox = _AABBEntities[i].GetComponent<AABB>();
            PhysicsBody entityPhysicsBody = _AABBEntities[i].GetComponent<PhysicsBody>();
            Transform entityTransform = _AABBEntities[i].GetComponent<Transform>();

            if (entityPhysicsBody.isStatic)
                continue;

            Vector3 velocity = entityPhysicsBody.velocity;

            // -------- Y AXIS --------
            entityTransform.position = entityTransform.position with
            {
                Y = entityTransform.position.Y + velocity.Y * deltaTime
            };

            entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
            entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;

            for (int e = 0; e < _currentSystemAmount; e++)
            {
                if (_AABBEntities[e].id == _AABBEntities[i].id)
                    continue;

                AABB other = _AABBEntities[e].GetComponent<AABB>();

                if (other.isTrigger)
                    continue;

                if (Intersects(entityBoundingBox, other))
                {
                    if (velocity.Y > 0)
                        entityTransform.position = entityTransform.position with
                        {
                            Y = other.min.Y - entityBoundingBox.maxOffset.Y
                        };
                    else if (velocity.Y < 0)
                        entityTransform.position = entityTransform.position with
                        {
                            Y = other.max.Y + entityBoundingBox.minOffset.Y // fixed
                        };

                    velocity.Y = 0;

                    entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
                    entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;
                }
            }

            // -------- X AXIS --------
            entityTransform.position = entityTransform.position with
            {
                X = entityTransform.position.X + velocity.X * deltaTime
            };

            entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
            entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;

            for (int e = 0; e < _currentSystemAmount; e++)
            {
                if (_AABBEntities[e].id == _AABBEntities[i].id)
                    continue;

                AABB other = _AABBEntities[e].GetComponent<AABB>();

                if (other.isTrigger)
                    continue;

                if (Intersects(entityBoundingBox, other))
                {
                    if (velocity.X > 0)
                        entityTransform.position = entityTransform.position with
                        {
                            X = other.min.X - entityBoundingBox.maxOffset.X
                        };
                    else if (velocity.X < 0)
                        entityTransform.position = entityTransform.position with
                        {
                            X = other.max.X + entityBoundingBox.minOffset.X // fixed
                        };

                    velocity.X = 0;

                    entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
                    entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;
                }
            }

            // -------- Z AXIS --------
            entityTransform.position = entityTransform.position with
            {
                Z = entityTransform.position.Z + velocity.Z * deltaTime
            };

            entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
            entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;

            for (int e = 0; e < _currentSystemAmount; e++)
            {
                if (_AABBEntities[e].id == _AABBEntities[i].id)
                    continue;

                AABB other = _AABBEntities[e].GetComponent<AABB>();

                if (other.isTrigger)
                    continue;

                if (Intersects(entityBoundingBox, other))
                {
                    if (velocity.Z > 0)
                        entityTransform.position = entityTransform.position with
                        {
                            Z = other.min.Z - entityBoundingBox.maxOffset.Z
                        };
                    else if (velocity.Z < 0)
                        entityTransform.position = entityTransform.position with
                        {
                            Z = other.max.Z + entityBoundingBox.minOffset.Z // fixed
                        };

                    velocity.Z = 0;

                    entityBoundingBox.min = entityTransform.position - entityBoundingBox.minOffset; // fixed
                    entityBoundingBox.max = entityTransform.position + entityBoundingBox.maxOffset;
                }
            }

            entityPhysicsBody.velocity = velocity;

            _AABBEntities[i].SetComponent(entityPhysicsBody);
            _AABBEntities[i].SetComponent(entityBoundingBox);
            _AABBEntities[i].SetComponent(entityTransform);
        }
    }
    
    bool Intersects(AABB a, AABB b)
    {
        return (a.min.X < b.max.X && a.max.X > b.min.X) &&
               (a.min.Y < b.max.Y && a.max.Y > b.min.Y) &&
               (a.min.Z < b.max.Z && a.max.Z > b.min.Z);
    }

    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _AABBEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _AABBEntities.CopyTo(updatedArray, 0);
            _AABBEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _AABBEntities[id] = e;
        _currentSystemAmount++;
    }
}