using Galaxy3D.Assets;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Gameplay;
using Galaxy3D.ECS.Components.Physics;
using ImGuiNET;
using OpenTK.Mathematics;

namespace Galaxy3D.SceneGraph.EngineEditorUI;

public class ComponentsWindow : EditorUI
{
    Entity _selectedEntity;
    
    public ComponentsWindow()
    {
        _tabName = "Inspector";
    }

    public override void DrawUI()
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(800, 0), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(800, 0), ImGuiCond.FirstUseEver);

        if (ImGui.Begin(_tabName))
        {
            if (_selectedEntity is null)
            {
                ImGui.TextDisabled("Select an entity to see properties");
                ImGui.End();
            }
            else
            {
                
                string name = _selectedEntity.entityName;
                
                if (ImGui.InputText("##name", ref name, 24))
                {
                    _selectedEntity.entityName = name;
                }
                ImGui.Separator();

                if (_selectedEntity.HasComponent<Transform>())
                {
                    if (ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        Transform transform = _selectedEntity.GetComponent<Transform>();
                    
                        transform.position = DrawVector3Control("Position", transform.position);
                        transform.rotation = DrawVector3Control("Rotation", transform.rotation);
                        transform.scale = DrawVector3Control("Scale", transform.scale);
                   
                        _selectedEntity.SetComponent(transform);
                    }
                }
                
                if (_selectedEntity.HasComponent<Camera>())
                {
                    if (ImGui.CollapsingHeader("Camera", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        Camera camera = _selectedEntity.GetComponent<Camera>();
                        
                        float fov = camera.fov;
                        if (ImGui.DragFloat("Field of View(FOV)", ref fov, 0.1f))
                        {
                            camera.fov = MathHelper.DegreesToRadians(fov);
                        }
                        
                        _selectedEntity.SetComponent(camera);
                    }
                }

                
                if (_selectedEntity.HasComponent<PhysicsBody>())
                {
                    if (ImGui.CollapsingHeader("PhysicsBody", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        PhysicsBody body = _selectedEntity.GetComponent<PhysicsBody>();
                        
                        bool isStatic = body.isStatic;
                        if (ImGui.Checkbox("IsStatic", ref isStatic))
                        {
                            body.isStatic = isStatic;
                        }

                        ImGui.Spacing();
                        
                        body.velocity = DrawVector3Control("Velocity", body.velocity);
                        
                        _selectedEntity.SetComponent(body);
                    }
                }
                
                if (_selectedEntity.HasComponent<PlayerMovement>())
                {
                    if (ImGui.CollapsingHeader("PlayerMovement", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        PlayerMovement pm = _selectedEntity.GetComponent<PlayerMovement>();
                        
                        float movementSpeed = pm.movementSpeed;
                        if (ImGui.DragFloat("Movement Speed", ref movementSpeed, 0.1f))
                        {
                            pm.movementSpeed = movementSpeed;
                        }
                        
                        float jumpForce = pm.jumpForce;
                        if (ImGui.DragFloat("Jump Force", ref jumpForce, 0.1f))
                        {
                            pm.jumpForce = jumpForce;
                        }
                        
                        _selectedEntity.SetComponent(pm);
                    }
                }
                
                if (_selectedEntity.HasComponent<AABB>())
                {
                    if (ImGui.CollapsingHeader("BoundingBox", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        AABB box = _selectedEntity.GetComponent<AABB>();
                        
                                                
                        bool isTrigger = box.isTrigger;
                        if (ImGui.Checkbox("IsTrigger", ref isTrigger))
                        {
                            box.isTrigger = isTrigger;
                        }
                        
                        box.posOffset = DrawVector3Control("Position Offset", box.posOffset);
                        box.max = DrawVector3Control("Max", box.max);
                        box.min = DrawVector3Control("Min", box.min);
                        
                        box.maxOffset = DrawVector3Control("Max Offset", box.maxOffset);
                        box.minOffset = DrawVector3Control("Min Offset", box.minOffset);
                        
                        _selectedEntity.SetComponent(box);
                    }
                }
                
                if (_selectedEntity.HasComponent<Material>())
                {
                    if (ImGui.CollapsingHeader("Material", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        Material material = _selectedEntity.GetComponent<Material>();
                        
                        // --- Color Edit ---
                        System.Numerics.Vector4 col = new (
                            material.materialColor.X, 
                            material.materialColor.Y, 
                            material.materialColor.Z, 
                            material.materialColor.W);

                        ImGui.Text("Diffuse Color");
                        if (ImGui.ColorEdit4("##matColor", ref col))
                        {
                            material.materialColor = new Vector4(col.X, col.Y, col.Z, col.W);
                            _selectedEntity.SetComponent(material);
                        }
                        
                        ImGui.Spacing();
                        ImGui.Separator();
                        ImGui.Text("Main Texture");
                        
                        IntPtr textureId = (IntPtr)material.texture.handle; 
                        System.Numerics.Vector2 imageSize = new (64, 64);
                        
                        ImGui.BeginGroup(); 
                        ImGui.Image(textureId, imageSize, new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));

                        ImGui.SameLine();
                        ImGui.BeginGroup();
                        ImGui.Text($"Res: ({material.texture._textureResolution.X}, {material.texture._textureResolution.Y})");
                        
                        // --- Texture Atlas Logic ---
                        if (material.texture is TextureAtlas atlas)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.4f, 0.9f, 0.4f, 1.0f));
                            ImGui.Text("Atlas Detected");
                            ImGui.PopStyleColor();

                            // Create the dropdown for Atlas IDs
                            int currentId = material.atlasID; 
                            string previewValue = $"Texture ID: {currentId}";

                            ImGui.SetNextItemWidth(120);
                            if (ImGui.BeginCombo("##AtlasDrop", previewValue))
                            {
                                // atlas.numberOfTexture comes from your TextureAtlas class
                                for (int i = 0; i < atlas.numberOfTexture; i++)
                                {
                                    bool isSelected = (currentId == i);
                                    if (ImGui.Selectable($"ID {i}", isSelected))
                                    {
                                        material.previousID = material.atlasID;
                                        material.atlasID = i;
                                        _selectedEntity.SetComponent(material);
                                        
                                        Console.WriteLine($"Material Atlas ID changed to: {i}");
                                    }

                                    if (isSelected)
                                        ImGui.SetItemDefaultFocus();
                                }
                                ImGui.EndCombo();
                            }
                        }
                        else
                        {
                            ImGui.Text("Standard Texture");
                        }
                        ImGui.EndGroup();
                        ImGui.EndGroup();
                        
                        ImGui.Spacing();
                        ImGui.Text("Shininess");
                        float shininess = material.shine;
                        if (ImGui.SliderFloat("##shininess", ref shininess, 1f, 256f))
                        {
                            material.shine = shininess;
                            _selectedEntity.SetComponent(material);
                        }
                    }   
                }

                if (_selectedEntity.HasComponent<DirectionalLight>())
                {
                    DirectionalLight light = _selectedEntity.GetComponent<DirectionalLight>();
                    System.Numerics.Vector4 col = new (
                        light.lightColor.X, 
                        light.lightColor.Y, 
                        light.lightColor.Z, 
                        1f);

                    ImGui.Text("Light Color");
                    if (ImGui.ColorEdit4("##lightColor", ref col))
                    {
                        light.lightColor = new Vector3(col.X, col.Y, col.Z);
                        _selectedEntity.SetComponent(light);
                    }
                    ImGui.Spacing();
                }
                
                if (_selectedEntity.HasComponent<MeshRenderer>())
                {
                    if(ImGui.CollapsingHeader("Mesh Renderer", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        MeshRenderer meshRenderer = _selectedEntity.GetComponent<MeshRenderer>();
                        ImGui.Text($"Total Vertices: {meshRenderer.meshData.Length / 3}");
                        ImGui.Text($"Total Indices: {meshRenderer.indices.Length}");
                        ImGui.Separator();
                    }
                }
                
                ImGui.End();
            }
        }
    }
    
    private Vector3 DrawVector3Control(string label, Vector3 value)
    {
        ImGui.PushID(label);
        ImGui.Columns(2);
        ImGui.SetColumnWidth(0, 80);
        ImGui.Text(label);
        ImGui.NextColumn();
        
        System.Numerics.Vector3 sysVec = new System.Numerics.Vector3(value.X, value.Y, value.Z);
        
        if (ImGui.DragFloat3("##v", ref sysVec, 0.1f))
        {
            value.X = sysVec.X;
            value.Y = sysVec.Y;
            value.Z = sysVec.Z;
        }

        ImGui.Columns(1);
        ImGui.PopID();
        
        return value;
    }
    
    public Entity selectedEntity { get => _selectedEntity; set => _selectedEntity = value; }
}