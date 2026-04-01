using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Gameplay;
using Galaxy3D.ECS.Components.Physics;
using Galaxy3D.ECS.Systems;
using Galaxy3D.EngineSpecific;
using Galaxy3D.EngineSpecific.ECS.Systems.Rendering;
using Galaxy3D.EngineSpecific.PhysicsAdditions;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D.ECS;
/// <summary>
/// Manages the entity flow in the system and is the representation of the whole ECS(u can make more ECS systems with each world)
/// populates each entity with an id that will be the entities refrence for each of its components in the system
/// 
/// </summary>
public class ECSWorld
{
    //The max amount of entities the system can holdw
    static int _maxEntities = 5000;
    IdGenerator _entityIdGenerator = new IdGenerator(5000);
    
    //Need to make it so that the arrays created doubles when its full
    Entity[] _entities = new Entity[5000];
    MeshSystem _meshSystem = new MeshSystem(_maxEntities);
    AnimationSystem _animationSystem = new AnimationSystem(_maxEntities);
    TransformSystem _transformSystem = new TransformSystem(_maxEntities);
    SkyboxSystem _skyboxSystem = new SkyboxSystem(_maxEntities);
    AABBSystem _aabbSystem = new AABBSystem(_maxEntities);
    PhysicsSystem _physicsSystem = new PhysicsSystem(_maxEntities, -3);
    PlayerMovementSystem _playerMovementSystem = new PlayerMovementSystem(_maxEntities);
    CameraSystem _cameraSystem = new CameraSystem(_maxEntities);
    Raycast engineRaycast = new Raycast();
    DebugDraw debugDraw = new DebugDraw();
    
    List<Entity> _newEntities = new List<Entity>();
    Dictionary<string, int> _entityLookup = new Dictionary<string, int>();
    
    
    //Used to keep track of the amount of entities currently in the system
    //Used to stop for loops from parsing through the entire list
    private int _currentEntityAmount = 0;
    #region Entity Methods
        //Creates an entity in the system as well as gives it an id
        public void CreateEntity(string entityName)
        {
            Console.WriteLine($"Creating entity {entityName}");
            if (_currentEntityAmount == _maxEntities)
            {
                _maxEntities += 100;
                _entityIdGenerator.maxAmountOfIds = _maxEntities;
                var updatedArray = new Entity[_maxEntities];
                _entities.CopyTo(updatedArray, 0);
                _entities = updatedArray;
            }
            
            int id = _entityIdGenerator.CreateEntityID();
            if (_entityLookup.ContainsKey(entityName))
                entityName = entityName + "_copy"+id;
            Entity newEntity = new Entity(id, entityName); 
            _entityLookup.Add(entityName, id);
            _entities[id] = newEntity;
            _currentEntityAmount++;
        }
        
        //Deletes the entity from the system and repopulates the que with the ID to be reused
        public void DeleteEntity(int entityID)
        {
            _entityIdGenerator.ReleaseEntityID(_entities[entityID].id);
            _entities[entityID] = null;
            _currentEntityAmount--;
        }
        
        public Entity GetEntity(int entityID)
        {
            return _entities[entityID];
        }
        
        public Entity GetEntity(string entityName)
        {
            return _entities[_entityLookup[entityName]];
        }
        
        public bool EntityExists(string entityName)
        {
            return _entities.Any(e => e != null && e.entityName == entityName);
        }
        //Returns an array with exactly the amount of entities for UI
        public Entity[] GetSceneArray()
        {
            Entity[] sceneEntity = new Entity[_currentEntityAmount];

            for (int i = 0; i < _currentEntityAmount; i++)
            {
                sceneEntity[i] = _entities[i];
            }
            
            return sceneEntity;
        }
    #endregion
    
    #region Systems

        public void PopulateSystems()
        {
            for (int i = 0; i < _currentEntityAmount; i++)
            {
                if (_entities[i] is not null && _entities[i].alreadySorted == false)
                {
                    if (_entities[i].HasComponent<Transform>())
                    {
                        _transformSystem.AddEntityToSystem(_entities[i]);
                    }

                    if (_entities[i].HasComponent<Camera>())
                    {
                        _cameraSystem.AddEntityToSystem(_entities[i]);
                        _meshSystem.camera = _entities[i];
                        debugDraw.camera = _entities[i];
                        _skyboxSystem.camera = _entities[i];
                    }

                    if (_entities[i].HasComponent<AABB>())
                    {
                        _aabbSystem.AddEntityToSystem(_entities[i]);
                        engineRaycast.colliders.Add(_entities[i]);
                    }

                    if (_entities[i].HasComponent<PlayerMovement>())
                    {
                        _playerMovementSystem.AddEntityToSystem(_entities[i]);
                    }
                    
                    if (_entities[i].HasComponent<PhysicsBody>())
                    {
                        _physicsSystem.AddEntityToSystem(_entities[i]);
                    }
                    
                    if (_entities[i].HasComponent<DirectionalLight>() ||_entities[i].HasComponent<MeshRenderer>() )
                    {
                        _meshSystem.AddEntityToSystem(_entities[i]);
                    }
                    if (_entities[i].HasComponent<Animator>())
                    {
                        _animationSystem.AddEntityToSystem(_entities[i]);
                    }

                    if (_entities[i].HasComponent<Skybox>())
                    {
                        _skyboxSystem.AddEntityToSystem(_entities[i]);
                    }
                    
                    _entities[i].alreadySorted = true;
                }
            }
        }

        public void LoadSystems()
        {
            _aabbSystem.LoadSystem();
            _meshSystem.LoadSystem();
            _skyboxSystem.LoadSystem();
            debugDraw.Load();

        }
        
        public void UseSystems(float deltaTime, KeyboardState input, MouseState mouseState)
        {
            _physicsSystem.UpdateSystem(deltaTime);
            _playerMovementSystem.UpdateSystem(input);
            _aabbSystem.UpdateSystem(deltaTime);
            _cameraSystem.UpdateSystem(mouseState);
            _transformSystem.UpdateSystem();
            _animationSystem.UpdateSystem(deltaTime, this);
            _meshSystem.UpdateSystem();
            _skyboxSystem.UpdateSystem();
            debugDraw.PersistantDraw();
            debugDraw.RenderLines();
   
        }
    #endregion
    
    public Raycast raycast {get => engineRaycast;}
    public DebugDraw draw {get => debugDraw;}
}