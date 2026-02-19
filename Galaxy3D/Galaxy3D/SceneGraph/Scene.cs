using Galaxy3D.ECS;
using Galaxy3D.EngineSpecific;

namespace Galaxy3D.SceneGraph;

/// <summary>
/// - This will be a template for all the scenes for the engine
///  
/// - A "game" scene will inherit this script
///     - The scene will contain a Tree data structure for easy retrieval and searching
///     - A scene will hold a ECS world
///     - A way to create entities from the scene
///     - A scene will also contain a scene graph which stores entities in a hierarchy
///     - Allow to track entities so only one entity will be made but will be instanced by the engine
/// </summary>
public class Scene
{
    static ECSWorld _sceneWorld = new ECSWorld(); //Creates an entity componenet system for the current scene
    SceneReader _reader = new SceneReader();
    private string _sceneFilePath;
    private string _sceneName;

    public Scene(string sceneFilePath)
    {
        _sceneFilePath = sceneFilePath;
        //Use the scene reader to read
    }
    
    public void LoadScene()
    {
        _sceneWorld.PopulateSystems();
        _sceneWorld.LoadSystems();
    }

    public void RunScene()
    {
        _sceneWorld.UseSystems();
    }
    
    
    public ECSWorld SceneWorld {get => _sceneWorld;}
}