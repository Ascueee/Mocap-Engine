using Galaxy3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Soul.ECS.Components;
using Galaxy3D.Assets;
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
        //Vertex data layout: First three vertex data, then the next two are UVs
        // Back face
        -0.5f, -0.5f, -0.5f, 0f, 0f,
        0.5f, -0.5f, -0.5f, 1f, 0f,
        0.5f,  0.5f, -0.5f, 1f, 1f,
        0.5f,  0.5f, -0.5f, 1f, 1f,
        -0.5f,  0.5f, -0.5f, 0f, 1f,
        -0.5f, -0.5f, -0.5f, 0f, 0f,
    
        // Front face
        -0.5f, -0.5f,  0.5f, 0f, 0f,
        0.5f, -0.5f,  0.5f, 1f, 0f,
        0.5f,  0.5f,  0.5f, 1f, 1f,
        0.5f,  0.5f,  0.5f, 1f, 1f,
        -0.5f,  0.5f,  0.5f, 0f, 1f,
        -0.5f, -0.5f,  0.5f, 0f, 0f,
    
        // Left face
        -0.5f,  0.5f,  0.5f, 1f, 1f,
        -0.5f,  0.5f, -0.5f, 0f, 1f,
        -0.5f, -0.5f, -0.5f, 0f, 0f,
        -0.5f, -0.5f, -0.5f, 0f, 0f,
        -0.5f, -0.5f,  0.5f, 1f, 0f,
        -0.5f,  0.5f,  0.5f, 1f, 1f,
    
        // Right face
        0.5f,  0.5f,  0.5f, 1f, 1f,
        0.5f,  0.5f, -0.5f, 0f, 1f,
        0.5f, -0.5f, -0.5f, 0f, 0f,
        0.5f, -0.5f, -0.5f, 0f, 0f,
        0.5f, -0.5f,  0.5f, 1f, 0f,
        0.5f,  0.5f,  0.5f, 1f, 1f,
    
        // Bottom face
        -0.5f, -0.5f, -0.5f, 0f, 1f,
        0.5f, -0.5f, -0.5f, 1f, 1f,
        0.5f, -0.5f,  0.5f, 1f, 0f,
        0.5f, -0.5f,  0.5f, 1f, 0f,
        -0.5f, -0.5f,  0.5f, 0f, 0f,
        -0.5f, -0.5f, -0.5f, 0f, 1f,
    
        // Top face
        -0.5f,  0.5f, -0.5f, 0f, 1f,
        0.5f,  0.5f, -0.5f, 1f, 1f,
        0.5f,  0.5f,  0.5f, 1f, 0f,
        0.5f,  0.5f,  0.5f, 1f, 0f,
        -0.5f,  0.5f,  0.5f, 0f, 0f,
        -0.5f,  0.5f, -0.5f, 0f, 1f,
        });
    
    Mesh triangleMesh = new Mesh(new float[]
    {
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // Bottom-left vertex
        0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // Bottom-right vertex
        0.0f,  0.5f, 0.0f, 0.5f, 1.0f  // Top vertex
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
        
    public Galaxy3D(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = (width, height), 
        Title = title,
        //WindowState = WindowState.Fullscreen
    }) { }

    protected override void OnLoad()
    {
        VoxelRegistry<Voxel> _voxelRegistry = new VoxelRegistry<Voxel>();
        _voxelRegistry.RegisterVoxel(0, new Voxel()); //registers a basic voxel with only a id
        
        _world.CreateEntity("Cube"); 
        _world.GetEntity(0).AddComponent(new Material(
            testShader, 
            atlas, 
            new Vector4(1.0f, 0.5f, 0.5f, 1.0f))
        );
        _world.GetEntity(0).AddComponent(new MeshRenderer(cubeMesh));
        
        _world.CreateEntity("World Generator");
        _world.GetEntity(1).AddComponent(new VoxelWorld<Voxel>(
            1,
            16,
            16,
            new VoxelRegistry<Voxel>())
        );
        
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