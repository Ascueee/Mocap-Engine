using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Systems;
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
    int _maxEntities = 800;
    IdGenerator _entityIdGenerator = new IdGenerator(800);
    
    //Need to make it so that the arrays created doubles when its full
    Entity[] _entities = new Entity[800];
    MeshSystem _meshSystem = new MeshSystem(800);
    Dictionary<string, int> _entityLookup = new Dictionary<string, int>();
    private DebugCamera testCam;
    
    //Used to keep track of the amount of entities currently in the system
    //Used to stop for loops from parsing through the entire list
    private int _currentEntityAmount = 0;
    
    #region Entity Methods
        //Creates an entity in the system as well as gives it an id
        public void CreateEntity(string entityName)
        {
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
                    if (_entities[i].HasComponent<DirectionalLight>())
                    {
                        Console.WriteLine("A light has been added to system: " + _entities[i].entityName);
                        _meshSystem.AddEntityToSystem(_entities[i]);
                    }
                    
                    if (_entities[i].HasComponent<MeshRenderer>())
                    {
                        _meshSystem.AddEntityToSystem(_entities[i]);
                    }
                }
            }
        }

        public void LoadSystems()
        {
            _meshSystem.LoadSystem();
            testCam = new DebugCamera(Vector3.UnitZ * 3, 800 / (float)600);
            _meshSystem.renderCamera = testCam;

        }

        public void UseSystems()
        {
            //transform
            //camera system
            //render system should be updates last
            _meshSystem.UpdateSystem();
            
        }
        
        //NOTE TO SELF DELETE THESE LATER:
        //TODO: Add a camera system!!
        public void TestCameraMove(FrameEventArgs e, KeyboardState input, MouseState mouse)
        {
            _meshSystem.renderCamera.MoveAround(input, e);
            _meshSystem.renderCamera.RotateCamera(mouse);
        }
        
    
    #endregion
    
}