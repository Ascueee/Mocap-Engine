using Galaxy3D.ECS;
namespace Galaxy3D.SceneGraph;
public class Scene
{
    static ECSWorld _sceneWorld = new ECSWorld(); //Creates an entity componenet system for the current scene
    private string _sceneFilePath; //This holds the file path where the scene save file is(this is to retrieve the scene ltr)
    private string _sceneName; //Naming of the specific scene

    public Scene(string sceneName)
    {
        _sceneName = sceneName;
        //Use the scene reader to read
    }
    
    
    
    
    public ECSWorld SceneWorld {get => _sceneWorld;}
    public string SceneFilePath {get => _sceneFilePath; set => _sceneFilePath = value; }
    public string SceneName {get => _sceneName;}
}