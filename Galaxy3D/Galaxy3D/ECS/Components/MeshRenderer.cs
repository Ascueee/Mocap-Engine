using Galaxy3D.Assets;
using Galaxy3D.ECS.Components;

namespace Soul.ECS.Components;

/// <summary>
/// Takes in mesh data and stores it for the render system
/// </summary>
public struct MeshRenderer : IComponent
{
    private int _componentID;
    private float[] _meshData;
    private Material _material;
    private int _vbo;
    private int _uvbo;
    private int _vao;
    public MeshRenderer(Mesh modelMesh)
    {
        _meshData = modelMesh.vertices; 
    }
    
    public int componentID { get; set; }
    
    public float[] meshData { get => _meshData; set => _meshData = value; }
    public int vbo { get => _vbo; set => _vbo = value; }
    public int uvbo { get => _uvbo; set => _uvbo = value; }
    public int vao { get => _vao; set => _vao = value; }
}