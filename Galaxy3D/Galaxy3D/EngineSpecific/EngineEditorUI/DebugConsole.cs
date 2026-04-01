using Galaxy3D.ECS;
using Galaxy3D.ECS.Components.Physics;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
namespace Galaxy3D.SceneGraph.EngineEditorUI;

public class DebugConsole : EditorUI
{
    List<string> _consoleLines = new List<string>();
    private string _input = ""; 
    Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
    bool _isOpen = false;
    private ECSWorld _world;

    public DebugConsole(ECSWorld world)
    {
        _world = world;
        RegisterCommand();
    }
    
    
    public override void DrawUI()
    {
        if (!_isOpen) return;

        ImGui.Begin("Console");

        foreach (var line in _consoleLines)
            ImGui.Text(line);
        
        ImGui.SetKeyboardFocusHere();
        
        if (ImGui.InputText("Input", ref _input, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            RunCommand(_input);
            _input = "";
        }

        ImGui.End();
    }
    
    public void RegisterCommand()
    {
        commands["clear"] = (args) =>
        {
            _consoleLines.Clear();
        };

        commands["wireframe"] = (args) =>
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _consoleLines.Add("Wireframe enabled");
        };
        
        commands["DrawAABB"] = (args) =>
        {
            // DrawAABB (string)entityName
            string entityName = args[0];
            _world.draw.AABBColliders.Add(_world.GetEntity(entityName));
        };
    }
    
    public void RunCommand(string line)
    {
        _consoleLines.Add("> " + line);
        
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            ImGui.SetScrollHereY(1.0f);
        
        var parts = line.Split(' ');

        string command = parts[0];
        string[] args = parts.Skip(1).ToArray();

        if (commands.ContainsKey(command))
            commands[command](args);
        else
            _consoleLines.Add("Unknown command");
    }
    
    public bool isOpen { get => _isOpen; set => _isOpen = value; }
}