using Galaxy3D.Assets;
using Galaxy3D.ECS.Components;

namespace Soul.ECS.Components;

/// <summary>
/// Takes in mesh data and stores it for the render system
/// </summary>
public struct MeshRenderer : IComponent
{
    private int _componentID;
    private float[] _vertexData;
    private float[] _uvData;
    private float[] _renderMeshData;
    private Material _material;
    private int _vbo;
    private int _vao;
    public MeshRenderer(Mesh modelMesh)
    {
        _vertexData = modelMesh.vertices;
        _uvData = modelMesh.uvs;
    }
    
    public int componentID { get; set; }
    
    public float[] meshData { get => _vertexData; set => _vertexData = value; }
    public float[] uvData { get => _uvData; set => _uvData = value; }
    public float[] renderMeshData { get => _renderMeshData; set => _renderMeshData = value; }
    public int vbo { get => _vbo; set => _vbo = value; }
    public int vao { get => _vao; set => _vao = value; }
}