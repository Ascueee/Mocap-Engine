using Galaxy3D.Assets;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Physics;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Vector2 = System.Numerics.Vector2;

namespace Galaxy3D.SceneGraph.EngineEditorUI;

public class LevelEditorViewport : EditorUI
{
    private Scene _activeScene;
    private GameWindow _window;

    private int cubeCounter;
    private TextureAtlas  baseWhiteTexture = new TextureAtlas(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/white.png",
        1,
        new OpenTK.Mathematics.Vector2(1,1)
    );
    
    private TextureAtlas  blockTextures = new TextureAtlas(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/BlockTextures.png",
        16,
        new OpenTK.Mathematics.Vector2(48,16)
    );
    
    Shader lightShader = new Shader(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.vert",
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.frag");

    public LevelEditorViewport(Scene scene, GameWindow window)
    {
        _activeScene = scene;
        _window = window;
        _tabName = "ViewportOverlay";
    }

    public override unsafe void DrawUI()
    {
        // Overlay covering the whole screen
        ImGui.SetNextWindowPos((Vector2)OpenTK.Mathematics.Vector2.Zero);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(_window.Size.X, _window.Size.Y));
        
        // Transparent and ignore mouse clicks (so you can still interact with the game)
        // unless you are dragging something!
        var flags = ImGuiWindowFlags.NoTitleBar | 
                    ImGuiWindowFlags.NoResize | 
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoScrollbar | 
                    ImGuiWindowFlags.NoBackground | 
                    ImGuiWindowFlags.NoBringToFrontOnFocus;

        ImGui.Begin(_tabName, flags);

        // Dummy to fill the space
        ImGui.Dummy(ImGui.GetContentRegionAvail());

        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("GALAXY_ASSET");
            if (payload.NativePtr != null)
            {
                var mousePos = ImGui.GetMousePos();
                OnItemDropped(mousePos, AssetLibrary.LevelEditorState.DraggingItem);
            }
            ImGui.EndDragDropTarget();
        }

        ImGui.End();
    }

    private void OnItemDropped(System.Numerics.Vector2 mousePos, string itemName)
    {
        string uniqueName = $"{itemName}_{cubeCounter}";
        _activeScene.SceneWorld.CreateEntity(uniqueName);
        
        Entity newEnt = _activeScene.SceneWorld.GetEntity(uniqueName);
        
        Entity camEntity = _activeScene.SceneWorld.GetEntity("Camera");
        Camera cam = camEntity.GetComponent<Camera>();

        float x = (2.0f * mousePos.X) / _window.Size.X - 1.0f;
        float y = 1.0f - (2.0f * mousePos.Y) / _window.Size.Y;

        Matrix4 invVP = Matrix4.Invert(cam.viewMatrix * cam.projectionMatrix);
        Vector4 near = new Vector4(x, y, -1.0f, 1.0f) * invVP;
        Vector4 far = new Vector4(x, y, 1.0f, 1.0f) * invVP;
        Vector3 rayStart = near.Xyz / near.W;
        Vector3 rayEnd = far.Xyz / far.W;
        Vector3 rayDir = Vector3.Normalize(rayEnd - rayStart);

        float t = -rayStart.Y / rayDir.Y;
        Vector3 spawnPos = rayStart + (rayDir * t);
        
        newEnt.AddComponent(new Transform(spawnPos, Vector3.Zero, Vector3.One));

        if (itemName == "Cube")
        {
            newEnt.AddComponent(new MeshRenderer(Mesh.cubeMesh));
            newEnt.AddComponent(new Material(
                lightShader,
                baseWhiteTexture,
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                0.3f,
                23f, 0)
            );
            newEnt.AddComponent(new AABB());
            newEnt.AddComponent(new PhysicsBody());
            
        }
        
        _activeScene.SceneWorld.PopulateSystems();
        _activeScene.SceneWorld.LoadSystems();
        cubeCounter++;
        Console.WriteLine($"Successfully dropped {uniqueName} at {spawnPos}");
    }
}