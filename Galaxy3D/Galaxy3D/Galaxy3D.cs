using Galaxy3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Galaxy3D.Assets;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.SceneGraph;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D;
/// <summary>
/// GENERAL NOTES TO SELF:
/// THis is an ecs engine where entities are the backbone of the engine
/// Entities function in a ECS world with componenets
/// The engine is also broken up into Scenes where a text file is used to layout the scenes(entities, assets, and shii being used)
///     A scene is where all the entities will be stored that will be used for that current scene and logic
///     
/// </summary>
public class Galaxy3D : GameWindow
{
    static Scene _testScene = new Scene("TestScene");
    private double _gameTime;
    
    //Fill it with cube mesh data later
    //ASSETS NOW I NEED TO ADD NORMALS TO THE MODELS AS WELL AS BASIC MESHES
    Mesh cubeMesh = new Mesh(
        new float[]
        {
            // Back (-Z)
            -0.5f, -0.5f, -0.5f, 0f, 0f,  0f, 0f, -1f,
            0.5f, -0.5f, -0.5f, 1f, 0f,  0f, 0f, -1f,
            0.5f,  0.5f, -0.5f, 1f, 1f,  0f, 0f, -1f,
            -0.5f,  0.5f, -0.5f, 0f, 1f,  0f, 0f, -1f,
        
            // Front (+Z)
            -0.5f, -0.5f, 0.5f, 0f, 0f,   0f, 0f, 1f,
            0.5f, -0.5f, 0.5f, 1f, 0f,   0f, 0f, 1f,
            0.5f,  0.5f, 0.5f, 1f, 1f,   0f, 0f, 1f,
            -0.5f,  0.5f, 0.5f, 0f, 1f,   0f, 0f, 1f,

            // Left (-X)
            -0.5f, -0.5f, -0.5f, 0f, 0f,  -1f, 0f, 0f,
            -0.5f, -0.5f,  0.5f, 1f, 0f,  -1f, 0f, 0f,
            -0.5f,  0.5f,  0.5f, 1f, 1f,  -1f, 0f, 0f,
            -0.5f,  0.5f, -0.5f, 0f, 1f,  -1f, 0f, 0f,
        
            // Right (+X)
            0.5f, -0.5f, -0.5f, 0f, 0f,   1f, 0f, 0f,
            0.5f, -0.5f,  0.5f, 1f, 0f,   1f, 0f, 0f,
            0.5f,  0.5f,  0.5f, 1f, 1f,   1f, 0f, 0f,
            0.5f,  0.5f, -0.5f, 0f, 1f,   1f, 0f, 0f,
        
            // Bottom (-Y)
            -0.5f, -0.5f, -0.5f, 0f, 0f,   0f, -1f, 0f,
            0.5f, -0.5f, -0.5f, 1f, 0f,   0f, -1f, 0f,
            0.5f, -0.5f,  0.5f, 1f, 1f,   0f, -1f, 0f,
            -0.5f, -0.5f,  0.5f, 0f, 1f,   0f, -1f, 0f,

            // Top (+Y)
            -0.5f,  0.5f, -0.5f, 0f, 0f,   0f, 1f, 0f,
            0.5f,  0.5f, -0.5f, 1f, 0f,   0f, 1f, 0f,
            0.5f,  0.5f,  0.5f, 1f, 1f,   0f, 1f, 0f,
            -0.5f,  0.5f,  0.5f, 0f, 1f,   0f, 1f, 0f,
        }, 
        new uint[]
        {
            0, 3, 2, 2, 1, 0,    // Back 
            4, 5, 6, 6, 7, 4,    // Front
            8, 9, 10, 10, 11, 8, // Left
            12, 15, 14, 14, 13, 12, // Right 
            16, 17, 18, 18, 19, 16, // Bottom
            20, 23, 22, 22, 21, 20  // Top
        });
    
    Mesh triangleMesh = new Mesh(new float[]
    {
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // Bottom-left vertex
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // Bottom-right vertex
        0.0f,  0.5f, 0.0f, 0.5f, 1.0f  // Top vertex
    }, new uint[]
    {
        0,1,2
    });

    private Mesh squareMesh = new Mesh(new float[]
    {
        0.5f,  0.5f, 0.0f, 1.0f, 1.0f,  // top right
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f,  // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,  // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left 
    }, new uint[]
    {
        0, 1, 3, //first triangle
        1, 2, 3 //second to make the square
    });
    
    Shader testShader = new Shader(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/Base.vert",
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/Base.frag");
    
    Shader lightShader = new Shader(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.vert",
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.frag");
    
    private TextureAtlas atlas = new TextureAtlas(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/container2.png",
        500,
        new Vector2(500,500)
        );
    
    private TextureAtlas  baseWhiteTexture = new TextureAtlas(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/white.png",
        1,
        new Vector2(1,1)
    );
    
    // private Model modelTest = new Model(
    //     "TestModel",
    //     "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Models/sponza.obj",
    //     _testScene.SceneWorld);
        
    public Galaxy3D(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = (width, height), 
        Title = title,
        
        //WindowState = WindowState.Fullscreen
    }) { }

    protected override void OnLoad()
    {
        
        //IN THE FUTURE ENTITIES WILL BE MADE IN JSON FILES IN A SCENE JSON FILE
        //
        
        //Creates a simple cube that will render
        _testScene.SceneWorld.CreateEntity("Cube"); 
        //through code you can easily Add components there are multiple component types(each struct and system class has a description
        _testScene.SceneWorld.GetEntity("Cube").AddComponent(new Material(
            lightShader, 
            atlas, 
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            0.3f,
            23f)
        );
        _testScene.SceneWorld.GetEntity("Cube").AddComponent(new Transform
        (new Vector3(-3.0f, -10.0f, 0.0f),
            Vector3.Zero,
            new Vector3(4f)));
        _testScene.SceneWorld.GetEntity("Cube").AddComponent(new MeshRenderer(cubeMesh));
        
        _testScene.SceneWorld.CreateEntity("Cube Two"); 
        //through code you can easily Add components there are multiple component types(each struct and system class has a description
        _testScene.SceneWorld.GetEntity("Cube Two").AddComponent(new Material(
            lightShader, 
            atlas, 
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            0.3f,
            23f)
        );
        _testScene.SceneWorld.GetEntity("Cube Two").AddComponent(new Transform
        (new Vector3(0.0f, -10.0f, 0.0f),
            Vector3.Zero,
            new Vector3(1f)));
        _testScene.SceneWorld.GetEntity("Cube Two").AddComponent(new MeshRenderer(cubeMesh));
        
        //Light One
        _testScene.SceneWorld.CreateEntity("Directional Light"); 
        _testScene.SceneWorld.GetEntity("Directional Light").AddComponent(new Material(
            testShader, 
            baseWhiteTexture, 
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            0.2f,
            32f)
        );
        _testScene.SceneWorld.GetEntity("Directional Light").AddComponent(new Transform
        (new Vector3(-0.0f, -10.0f, -10.0f),
            Vector3.Zero,
            Vector3.One));
        _testScene.SceneWorld.GetEntity("Directional Light").AddComponent(new MeshRenderer(cubeMesh));
        _testScene.SceneWorld.GetEntity("Directional Light").AddComponent(new DirectionalLight(
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(1f),
            new Vector3(1f),
            new Vector3(1f)));
        
        //TODO: NEED TO RECHECK THIS
        //Now creates a parent to child relationship in the engine
        //this means that if the parent transforms is affected the childs transform is affected too
        // _testScene.SceneWorld.GetEntity("Cube").SetChild(_testScene.SceneWorld.GetEntity("Cube Two"));
        _testScene.LoadScene();
        GL.ClearColor(0.5f, 0.5f, 0.5f, 1f); 
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _testScene.RunScene();
        
        SwapBuffers();
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        _gameTime = e.Time;
        var input = KeyboardState;
        var mouseState = MouseState;
        _testScene.SceneWorld.TestCameraMove(e, input, mouseState);
        
        Transform transfrom = _testScene.SceneWorld.GetEntity("Cube").GetComponent<Transform>();
        // transfrom.position += new Vector3(3, 0, 0) * (float)_gameTime;
        transfrom.rotation += new Vector3(0.5f, 0, 0) * (float)_gameTime;
        _testScene.SceneWorld.GetEntity("Cube").SetComponent(transfrom);
        
        
        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }
}

public class EngineRunner()
{
    static void Main(string[] args)
    {
        using (Galaxy3D engine = new Galaxy3D(800, 600, "Galaxy3D"))
        {
            engine.Run();
        }
    }
}