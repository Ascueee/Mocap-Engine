using Galaxy3D.ECS;
using ImGuiNET;

namespace Galaxy3D.SceneGraph.EngineEditorUI;

public class SceneHierarchy : EditorUI
{
    private Entity[] _sceneEntities;
    private Scene _activeScene;
    private Entity _selectedEntity;
    public SceneHierarchy(Scene currentScene)
    {
        _tabName = "Scene Hierarchy: " + currentScene.SceneName;
        _activeScene = currentScene;
        
    }

    public override void DrawUI()
    {
        _sceneEntities = _activeScene.SceneWorld.GetSceneArray();
        
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(0, 0), ImGuiCond.FirstUseEver);
        
        if (ImGui.Begin(_tabName))
        {
            ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.7f, 1.0f, 1.0f), "Entities");
            ImGui.Separator();
            //Need to only display the entities without a parent(If they have a parent it means they are a child)
            foreach (var entity in _sceneEntities)
            {
                if(entity.parent is null)
                    DrawEntityOnHierarchy(entity);
            }
            
            if (ImGui.BeginPopupContextWindow("HierarchyContext"))
            {
                if (ImGui.MenuItem("Create Empty Entity"))
                {
                    _activeScene.SceneWorld.CreateEntity("New Entity");
                    _sceneEntities = _activeScene.SceneWorld.GetSceneArray();
                }
                ImGui.EndPopup();
            }
            ImGui.End();
        }
    }
    
    public void DrawEntityOnHierarchy(Entity entity)
    {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;
        if (_selectedEntity == entity) flags |= ImGuiTreeNodeFlags.Selected;
        if (entity.children.Count == 0) flags |= ImGuiTreeNodeFlags.Leaf;

        bool opened = ImGui.TreeNodeEx($"{entity.entityName}", flags);
        
        if (ImGui.IsItemClicked())
        {
            _selectedEntity = entity;
        }
        
        if (opened)
        {
            foreach (var child in entity.children)
            {
                DrawEntityOnHierarchy(child);
            }
            ImGui.TreePop();
        }
    }
    
    public Entity selectedEntity { get => _selectedEntity; set => _selectedEntity = value; }
}