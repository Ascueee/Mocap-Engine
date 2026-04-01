using Galaxy3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Galaxy3D.Assets;
using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Gameplay;
using Galaxy3D.ECS.Components.Physics;
using Galaxy3D.EngineSpecific;
using Galaxy3D.EngineSpecific.PhysicsAdditions;
using Galaxy3D.SceneGraph;
using Galaxy3D.SceneGraph.EngineEditorUI;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Galaxy3D;

/// <summary>
/// GENERAL NOTES TO SELF:
/// THis is an ecs engine where entities are the backbone of the engine
/// Entities function in a ECS world with components
/// The engine is also broken up into Scenes where a text file is used to layout the scenes(entities, assets, and shii being used)
///     A scene is where all the entities will be stored that will be used for that current scene and logic
/// </summary>


public enum EditorState
{
    Game,
    Editor,
}
public class Galaxy3D : GameWindow
{
    static Scene _testScene = new Scene("Test Scene");
    private double _gameTime;
    private SceneHierarchy _sceneHierarchy;
    private ComponentsWindow _componentsWindow;
    private ImGuiController _controller;
    AssetLibrary _assetLibrary;
    LevelEditorViewport _editorViewport;
    DebugConsole _debugConsole;
    EditorState _editorState = EditorState.Editor;

    #region Assets For Galaxy3D(Meshes, Shaders, Textures, Models, etc)
        Shader testShader = new Shader(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/Base.vert",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/Base.frag");
        
        Shader lightShader = new Shader(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.vert",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.frag");

        private Shader skyboxShader = new Shader("/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/SkyBox.vert",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/SkyBox.frag");
        
        private TextureAtlas atlas = new TextureAtlas(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/container2.png",
            500,
            new Vector2(500,500)
            );

        private Texture gridTex = new Texture("/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/grid.png");

        private Texture skyboxTexture = new Texture(new string[]
        {
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/Skybox/sky_06_cubemap_2k/px.png",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/Skybox/sky_06_cubemap_2k/nx.png",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/Skybox/sky_06_cubemap_2k/py.png",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/Skybox/sky_06_cubemap_2k/ny.png",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/Skybox/sky_06_cubemap_2k/pz.png",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/Skybox/sky_06_cubemap_2k/nz.png"
        });
        
        private TextureAtlas  baseWhiteTexture = new TextureAtlas(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/white.png",
            1,
            new Vector2(1,1)
        );
        
        private Model Humanoid = new Model(
            "Humanoid",
            ".fbx",
            _testScene.SceneWorld);
        
        // private Model sponza = new Model(
        //     "sponza",
        //     ".obj",
        //     _testScene.SceneWorld);
        
        private Model m1911 = new Model(
            "M1911",
            ".fbx",
            _testScene.SceneWorld);
        
        private Animation _testAnimationWalk = new Animation("WalkTest", 1f);
        Animation _testAnimationHoldItem = new Animation("HoldItemTest", 1f);
        Animation _testAnimationWave = new Animation("WaveTest", 1f);
        #endregion
                

    public Galaxy3D(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = (width, height), 
        Title = title,
        
        //WindowState = WindowState.Fullscreen
    }) { }

    protected override void OnLoad()
    {
        //Adds a arm swing in the arm animation
        _testAnimationWalk.AnimatedEntities.Add(
            "Humanoid_LeftArm",
            new List<Keyframe>()
            {
                new Keyframe(0.0f, new Vector3(0.0f, 0f, 0.8f)),
                new Keyframe(0.5f, new Vector3(0.0f, 0f, -0.8f)),
                new Keyframe(1f, new Vector3(0.0f, 0f, 0.8f)),
            }
        );
        
        _testAnimationWalk.AnimatedEntities.Add(
            "Humanoid_RightArm",
            new List<Keyframe>()
            {
                new Keyframe(0.0f, new Vector3(0.0f, 0f, -0.8f)),
                new Keyframe(0.5f, new Vector3(0.0f, 0f, 0.8f)),
                new Keyframe(1f, new Vector3(0.0f, 0f, -0.8f)),
            }
        );
        
        _testAnimationWalk.AnimatedEntities.Add(
            "Humanoid_LeftLeg",
            new List<Keyframe>()
            {
                new Keyframe(0.0f, new Vector3(0.0f, 0f, 0.8f)),
                new Keyframe(0.5f, new Vector3(0.0f, 0f, -0.8f)),
                new Keyframe(1f, new Vector3(0.0f, 0f, 0.8f)),
            }
        );
        
        _testAnimationWalk.AnimatedEntities.Add(
            "Humanoid_RightLeg",
            new List<Keyframe>()
            {
                new Keyframe(0.0f, new Vector3(0.0f, 0f, -0.8f)),
                new Keyframe(0.5f, new Vector3(0.0f, 0f, 0.8f)),
                new Keyframe(1f, new Vector3(0.0f, 0f, -0.8f)),
            }
        );
        
        _testAnimationHoldItem.AnimatedEntities.Add("Humanoid_RightArm", new List<Keyframe>()
        {
            new Keyframe(0.0f, new Vector3(0.0f, 0.0f, 1.2f)), // arm raised holding item
            new Keyframe(1.0f, new Vector3(0.0f, 0.0f, 1.2f)), // stays raised (same keyframe)
        });
        
        _testAnimationWave.AnimatedEntities.Add("Humanoid_LeftArm", new List<Keyframe>()
        {
            new Keyframe(0.0f, new Vector3(0.0f, 0.0f, 3f)),
            new Keyframe(0.25f, new Vector3(0.4f, 0.0f, 3f)),
            new Keyframe(0.5f, new Vector3(-0.4f, 0.0f, 3f)),
            new Keyframe(0.75f, new Vector3(0.4f, 0.0f, 3f)),
            new Keyframe(1.0f, new Vector3(0.0f, 0.0f, 3f))
        });
        
        _testAnimationWalk.isLooping = true;
        _testAnimationWave.isLooping = true;
        _controller = new ImGuiController(Size.X, Size.Y);
        
        #region Galaxy3D Entity SetUp
            
            
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
            _testScene.SceneWorld.GetEntity("Directional Light").AddComponent(new MeshRenderer(Mesh.cubeMesh));
            _testScene.SceneWorld.GetEntity("Directional Light").AddComponent(new DirectionalLight(
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f),
                new Vector3(0.5f),
                new Vector3(0.5f)));
            
            
            _testScene.SceneWorld.CreateEntity("PlayerEntity");
            _testScene.SceneWorld.GetEntity("PlayerEntity").AddComponent(new Transform());
            _testScene.SceneWorld.GetEntity("PlayerEntity").AddComponent(new PlayerMovement(3, 10));
            _testScene.SceneWorld.GetEntity("PlayerEntity").AddComponent(new PhysicsBody());
            _testScene.SceneWorld.GetEntity("PlayerEntity").AddComponent(new AABB());
            
            AABB playerAABB = _testScene.SceneWorld.GetEntity("PlayerEntity").GetComponent<AABB>();
            playerAABB.minOffset = new Vector3(1.0f, 2.0f, 1.0f);
            playerAABB.maxOffset = new Vector3(1.0f, 2.6f, 1.0f);
            _testScene.SceneWorld.GetEntity("PlayerEntity").SetComponent(playerAABB);
            
             PhysicsBody playerBody = _testScene.SceneWorld.GetEntity("PlayerEntity").GetComponent<PhysicsBody>();
             playerBody.isStatic = true;
            
            Transform playerTransform = _testScene.SceneWorld.GetEntity("PlayerEntity").GetComponent<Transform>();
            playerTransform.position = new Vector3(3.0f, 0.0f, 0.0f);
            //_testScene.SceneWorld.GetEntity("PlayerEntity").SetComponent(playerBody);
            _testScene.SceneWorld.GetEntity("PlayerEntity").SetComponent(playerTransform);
        
        
            _testScene.SceneWorld.CreateEntity("Camera");
            _testScene.SceneWorld.GetEntity("Camera").AddComponent(new Camera((float)800 / 600, 90f));
            _testScene.SceneWorld.GetEntity("Camera").AddComponent(new Transform());    
            _testScene.SceneWorld.GetEntity("PlayerEntity").SetChild(_testScene.SceneWorld.GetEntity("Camera"));
            
            
            _testScene.SceneWorld.CreateEntity("Skybox");
            _testScene.SceneWorld.GetEntity("Skybox").AddComponent(new Skybox());
            _testScene.SceneWorld.GetEntity("Skybox").AddComponent(new Transform());
            _testScene.SceneWorld.GetEntity("Skybox").AddComponent(new Material(
                skyboxShader, 
                skyboxTexture, 
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                0.2f,
                32f)
            );
            
            //_testScene.SceneWorld.GetEntity("Humanoid").AddComponent(new AABB());
            _testScene.SceneWorld.GetEntity("Humanoid").AddComponent(new PhysicsBody());
            _testScene.SceneWorld.GetEntity("Humanoid").AddComponent(new Animator());
        
            
            Animator animator = _testScene.SceneWorld.GetEntity("Humanoid").GetComponent<Animator>();
            animator.animations.Add("Walk", _testAnimationWalk);
            animator.animations.Add("HoldItem", _testAnimationHoldItem);
            animator.animations.Add("Wave", _testAnimationWave);
            animator.PlayOnLayer(0, "Walk", 1.5f);
            animator.PlayOnLayer(1, "Wave", 1f);
            animator.PlayOnLayer(2, "HoldItem", 1.5f);
            //animator.StopLayer(0);
        
            _testScene.SceneWorld.GetEntity("Humanoid").SetComponent(animator);
            Transform model = _testScene.SceneWorld.GetEntity("Humanoid").GetComponent<Transform>();
            model.scale = new Vector3(1f);
            _testScene.SceneWorld.GetEntity("Humanoid").SetComponent(model);
            
            // Transform sponza = _testScene.SceneWorld.GetEntity("sponza.obj").GetComponent<Transform>();
            // sponza.scale = new Vector3(0.1f, 0.1f, 0.1f);
            // _testScene.SceneWorld.GetEntity("sponza.obj").SetComponent(sponza);
            
            _testScene.SceneWorld.GetEntity("Camera").SetChild(_testScene.SceneWorld.GetEntity("M1911"));
            Transform m1911Trans = _testScene.SceneWorld.GetEntity("M1911").GetComponent<Transform>();
            m1911Trans.rotation = _testScene.SceneWorld.GetEntity("Camera").GetComponent<Transform>().rotation;
            _testScene.SceneWorld.GetEntity("M1911").SetComponent(m1911Trans);
        #endregion
        
        //Gives the engine the entities and Initializes all engine systems
        _testScene.SceneWorld.PopulateSystems();
        _testScene.SceneWorld.LoadSystems();
        
        _sceneHierarchy = new SceneHierarchy(_testScene);
        _componentsWindow = new ComponentsWindow();
        _debugConsole = new DebugConsole(_testScene.SceneWorld);
        _assetLibrary = new AssetLibrary();
        _editorViewport = new LevelEditorViewport(_testScene, this);
        
        GL.ClearColor(0.5f, 0.5f, 0.5f, 1f); 
    }
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Enable(EnableCap.DepthTest);
        // GL.Enable(EnableCap.CullFace);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        var input = KeyboardState;
        var mouseState = MouseState;
        
        _testScene.SceneWorld.UseSystems((float)e.Time, KeyboardState, MouseState);
        
        //Draws all the EDITOR UI(This uses ImGUI)
        if (_editorState == EditorState.Editor)
        {
            _controller.Begin();
            _sceneHierarchy.DrawUI();
            _componentsWindow.selectedEntity = _sceneHierarchy.selectedEntity;
            _componentsWindow.DrawUI();
            _debugConsole.DrawUI();
            _assetLibrary.DrawUI();
            _editorViewport.DrawUI();
            _controller.Render();
        }
        SwapBuffers();
    }
    
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        _gameTime = e.Time;
        
        var input = KeyboardState;
        var mouseState = MouseState;
        
        if (input.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (input.IsKeyDown(Keys.Tab))
        {
            this.CursorState = CursorState.Hidden;
            this.CursorState = CursorState.Grabbed;
            Camera cam = _testScene.SceneWorld.GetEntity("Camera").GetComponent<Camera>();
            cam.unfocused = true;
            _testScene.SceneWorld.GetEntity("Camera").SetComponent(cam);
        }

        if (input.IsKeyDown(Keys.CapsLock))
        {
            this.CursorState = CursorState.Normal;
            Camera cam = _testScene.SceneWorld.GetEntity("Camera").GetComponent<Camera>();
            cam.unfocused = false;
            _testScene.SceneWorld.GetEntity("Camera").SetComponent(cam);

        }

        if (input.IsKeyDown(Keys.Slash))
        {
            _debugConsole.isOpen = !_debugConsole.isOpen;
        }
        
        //Shoots A raycast
        _testScene.SceneWorld.raycast.Hit(_testScene.SceneWorld.GetEntity("Humanoid").GetComponent<Transform>().position,
            new Vector3(1.0f, 0.0f, 0.0f), 10);
        
        _testScene.SceneWorld.draw.DrawRay(_testScene.SceneWorld.GetEntity("Humanoid").GetComponent<Transform>().position,
            new Vector3(1.0f, 0.0f, 0.0f), 10f);
        
        
        
        _controller.Update(this, (float)_gameTime); 
    }
    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e); 
        
        GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
        _controller.WindowResized(Size.X, Size.Y);
    }
    
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        _controller.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        _controller.MouseScroll(e.Offset);
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