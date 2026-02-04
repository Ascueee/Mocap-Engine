using Galaxy3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Galaxy3D.Assets;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.EngineSpecific;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D;
/// <summary>
/// GENERAL NOTES TO SELF:
///     COMPONENTS ARE STRUCTS MAKE SURE TO RESET THEIR COMPONENTS USING THE SETCOMPONENT METHOD
/// </summary>
public class Galaxy3D : GameWindow
{
    static ECSWorld _world = new ECSWorld();
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
            0,1,2, 2,3,0,       // Back
            4,5,6, 6,7,4,       // Front
            8,9,10, 10,11,8,    // Left
            12,13,14, 14,15,12, // Right
            16,17,18, 18,19,16, // Bottom
            20,21,22, 22,23,20  // Top
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
    
    private Model modelTest = new Model(
        "TestModel",
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Models/teapot.obj",
        _world);
        
    public Galaxy3D(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = (width, height), 
        Title = title,
        
        //WindowState = WindowState.Fullscreen
    }) { }

    protected override void OnLoad()
    {
        
        //Creates a simple cube that will render
        _world.CreateEntity("Cube"); 
        //through code you can easily Add components there are multiple component types(each struct and system class has a description
        _world.GetEntity("Cube").AddComponent(new Material(
            lightShader, 
            atlas, 
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            1f,
            40f)
        );
        _world.GetEntity("Cube").AddComponent(new Transform
        (new Vector3(0.0f, 1.0f, 0.0f),
            Vector3.Zero,
            new Vector3(4f)));
        _world.GetEntity("Cube").AddComponent(new MeshRenderer(cubeMesh));
        
        //Light One
        _world.CreateEntity("LightSource"); 
        _world.GetEntity("LightSource").AddComponent(new Material(
            testShader, 
            baseWhiteTexture, 
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            0.2f,
            32f)
        );
        _world.GetEntity("LightSource").AddComponent(new Transform
        (new Vector3(-1.0f, -1.0f, -0.5f),
            Vector3.Zero,
            Vector3.One));
        _world.GetEntity("LightSource").AddComponent(new MeshRenderer(cubeMesh));
        _world.GetEntity("LightSource").AddComponent(new DirectionalLight(
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.2f),
            new Vector3(0.5f),
            new Vector3(0.2f)));
        
        //Light Two
        _world.CreateEntity("LightSourceTwo"); 
        _world.GetEntity("LightSourceTwo").AddComponent(new Material(
            testShader, 
            baseWhiteTexture, 
            new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
            0.5f,
            32f)
        );
        _world.GetEntity("LightSourceTwo").AddComponent(new Transform
        (new Vector3(-1.0f, 0.0f, -0.5f),
            Vector3.Zero,
            Vector3.One));
        _world.GetEntity("LightSourceTwo").AddComponent(new MeshRenderer(cubeMesh));
        _world.GetEntity("LightSourceTwo").AddComponent(new DirectionalLight(
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.1f),
            new Vector3(0.5f),
            new Vector3(0.2f)));
        
        // //Light Three
        // _world.CreateEntity("LightSourceThree"); 
        // _world.GetEntity("LightSourceThree").AddComponent(new Material(
        //     testShader, 
        //     baseWhiteTexture, 
        //     new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        //     0.5f,
        //     32f)
        // );
        // _world.GetEntity("LightSourceThree").AddComponent(new Transform
        // (new Vector3(0.0f, 0.5f, 1.0f),
        //     Vector3.Zero,
        //     Vector3.One));
        // _world.GetEntity("LightSourceThree").AddComponent(new MeshRenderer(cubeMesh));
        // _world.GetEntity("LightSourceThree").AddComponent(new DirectionalLight(
        //     new Vector3(1.0f, 0.0f, 0.5f),
        //     new Vector3(0.1f),
        //     new Vector3(0.5f),
        //     new Vector3(0.2f)));
        //
        //Point light
        _world.CreateEntity("LightSourceFour"); 
        _world.GetEntity("LightSourceFour").AddComponent(new Material(
            testShader, 
            baseWhiteTexture, 
            new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            0.5f,
            32f)
        );
        _world.GetEntity("LightSourceFour").AddComponent(new Transform
        (new Vector3(0.0f, 15.0f, 0.0f),
            Vector3.Zero,
            Vector3.One));
        _world.GetEntity("LightSourceFour").AddComponent(new MeshRenderer(cubeMesh));
        _world.GetEntity("LightSourceFour").AddComponent(new PointLight(
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.2f),
            new Vector3(0.9f),
            new Vector3(0.5f), 
            1.0f, 
            0.045f, 
            0.0075f
            ));

        
        //TODO: NEED TO RECHECK THIS
        //Now creates a parent to child relationship in the engine
        //this means that if the parent transforms is affected the childs transform is affected too
        _world.GetEntity("Cube").SetChild(_world.GetEntity("Cube"));
        _world.GetEntity("Cube").SetChild(_world.GetEntity("LightSource"));
        
        //load the ecs and get it ready
        _world.PopulateSystems();
        _world.LoadSystems();
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Enable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _world.UseSystems();
        
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
        _world.TestCameraMove(e, input, mouseState);
        
        
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