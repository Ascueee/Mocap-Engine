using Soul.ECS.Components;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Systems;
using Galaxy3D.EngineSpecific;

namespace Galaxy3D;
/// <summary>
/// Manages the entity flow in the system and is the representation of the whole ECS(u can make more ECS systems with each world)
/// populates each entity with an id that will be the entities refrence for each of its components in the system
/// 
/// </summary>
public class ECSWorld
{
    //The max amount of entities the system can hold
    private const int MAX_ENTITIES = 1000000;
    IdGenerator _entityIdGenerator = new IdGenerator(MAX_ENTITIES);
    Entity[] _entities = new Entity[MAX_ENTITIES];
    MeshSystem _meshSystem = new MeshSystem(MAX_ENTITIES);
    
    //Used to keep track of the amount of entities currently in the system
    //Used to stop for loops from parsing through the entire list
    private int _currentEntityAmount = 0;
    
    #region Entity Methods
        //Creates an entity in the system as well as gives it an id
        public void CreateEntity(string entityName)
        {
            int id = _entityIdGenerator.Dequeue(); //removes id from que
            Entity newEntity = new Entity(id, entityName); 
            _entities[id] = newEntity;
            _currentEntityAmount++;
        }
        
        //Deletes the entity from the system and repopulates the que with the ID to be reused
        public void DeleteEntity(int entityID)
        {
            _entityIdGenerator.Enqueue(_entities[entityID].id);
            _entities[entityID] = null;
            _currentEntityAmount--;
        }
        
        public Entity GetEntity(int entityID)
        {
            return _entities[entityID];
        }
        
        public void PrintEntities()
        {
            for (int i = 0; i < _currentEntityAmount; i++)
            {
                if(_entities[i] is not null)
                    _entities[i].PrintComponents();
            }
        }
        
    #endregion
    
    #region Systems

        public void PopulateSystems()
        {
            for (int i = 0; i < _currentEntityAmount; i++)
            {
                if (_entities[i] is not null)
                {
                    if (_entities[i].HasComponent<MeshRenderer>())
                    {
                        Console.WriteLine("Adding to Mesh Renderer");
                        _meshSystem.AddEntityToSystem(_entities[i]);
                    }

                    if (_entities[i].HasComponent<VoxelWorld<Voxel>>())
                    {
                        
                    }
                }
            }
        }

        public void LoadSystems()
        {
            _meshSystem.LoadSystem();
            
        }

        public void UseSystems()
        {
            _meshSystem.UpdateSystem();
        }
    
    #endregion
    
}