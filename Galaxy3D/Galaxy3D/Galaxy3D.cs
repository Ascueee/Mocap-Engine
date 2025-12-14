using Galaxy3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Soul.ECS.Components;
using Galaxy3D.Assets;
using Galaxy3D.Assets.Models;
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
    //ASSETS
    Mesh cubeMesh = new Mesh(
        new float[]
    {
        -0.5f, -0.5f, -0.5f, 0f, 0f,
        0.5f, -0.5f, -0.5f, 1f, 0f,
        0.5f,  0.5f, -0.5f, 1f, 1f,
        -0.5f,  0.5f, -0.5f, 0f, 1f,
        
        -0.5f, -0.5f, 0.5f, 0f, 0f,
        0.5f, -0.5f, 0.5f, 1f, 0f,
        0.5f,  0.5f, 0.5f, 1f, 1f,
        -0.5f,  0.5f, 0.5f, 0f, 1f,

        -0.5f, -0.5f, -0.5f, 0f, 0f,
        -0.5f, -0.5f,  0.5f, 1f, 0f,
        -0.5f,  0.5f,  0.5f, 1f, 1f,
        -0.5f,  0.5f, -0.5f, 0f, 1f,
        
        0.5f, -0.5f, -0.5f, 0f, 0f,
        0.5f, -0.5f,  0.5f, 1f, 0f,
        0.5f,  0.5f,  0.5f, 1f, 1f,
        0.5f,  0.5f, -0.5f, 0f, 1f,
        
        -0.5f, -0.5f, -0.5f, 0f, 0f,
        0.5f, -0.5f, -0.5f, 1f, 0f,
        0.5f, -0.5f,  0.5f, 1f, 1f,
        -0.5f, -0.5f,  0.5f, 0f, 1f,

        -0.5f,  0.5f, -0.5f, 0f, 0f,
        0.5f,  0.5f, -0.5f, 1f, 0f,
        0.5f,  0.5f,  0.5f, 1f, 1f,
        -0.5f,  0.5f,  0.5f, 0f, 1f,
    }, new uint[]
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

    private Texture testTexture = new Texture(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/container2.png");

    private TextureAtlas atlas = new TextureAtlas(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/container2.png",
        500,
        new Vector2(500,500)
        );

    // private Model modelTest = new Model(
    //     "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Models/erato.obj",
    //     _world);    // private Model modelTest = new Model(
    //     "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Models/erato.obj",
    //     _world);
        
    public Galaxy3D(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = (width, height), 
        Title = title,
        //WindowState = WindowState.Fullscreen
    }) { }

    protected override void OnLoad()
    {
        
        _world.CreateEntity("Cube"); 
        _world.GetEntity("Cube").AddComponent(new Material(
            testShader, 
            atlas, 
            new Vector4(1.0f, 0.5f, 0.5f, 1.0f))
        );
        _world.GetEntity("Cube").AddComponent(new Transform());
        _world.GetEntity("Cube").AddComponent(new MeshRenderer(cubeMesh));
        
        
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