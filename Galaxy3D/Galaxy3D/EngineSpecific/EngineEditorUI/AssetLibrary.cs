using ImGuiNET;

namespace Galaxy3D.SceneGraph.EngineEditorUI;

public class AssetLibrary : EditorUI
{
    // A list of items we can spawn
    private string[] _prefabs = { "Cube", };

    public AssetLibrary()
    {
        _tabName = "Asset Library";
    }

    public override void DrawUI()
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 300), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(200, 300), ImGuiCond.FirstUseEver);

        if (ImGui.Begin(_tabName))
        {
            ImGui.TextColored(new System.Numerics.Vector4(0.2f, 0.8f, 0.2f, 1.0f), "Available Prefabs");
            ImGui.Separator();

            foreach (var item in _prefabs)
            {
                ImGui.Selectable(item);

                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload("GALAXY_ASSET", IntPtr.Zero, 0);
                    LevelEditorState.DraggingItem = item;
                    
                    ImGui.Text($"Spawning {item}...");
                    ImGui.EndDragDropSource();
                }
            }
            ImGui.End();
        }
    }
    
    public static class LevelEditorState 
    {
        public static string DraggingItem = "";
    }
}