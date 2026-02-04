using Galaxy3D.Assets;

namespace Galaxy3D.ECS.Components;

/// <summary>
/// Takes in mesh data and stores it for the render system
/// </summary>
public struct MeshRenderer : IComponent
{
    private int _componentID;
    private bool _isActive;
    private float[] _vertexData;
    private float[] _uvData;
    private float[] _normalData;
    private uint[] _indices;
    private float[] _renderMeshData;//recombined mesh data
    private Material _material;
    private int _vbo;
    private int _vao;
    private int _ebo;
    public MeshRenderer(Mesh modelMesh)
    {
        _vertexData = modelMesh.vertices;
        _uvData = modelMesh.uvs;
        _indices = modelMesh.indices;
        _normalData = modelMesh.normals;
    }
    
    public int componentID { get; set; }
    public bool isActive { get; set; }
    
    public float[] meshData { get => _vertexData; set => _vertexData = value; }
    public float[] uvData { get => _uvData; set => _uvData = value; }
    public float[] normalData { get => _normalData; set => _normalData = value; }
    public uint[] indices { get => _indices; set => _indices= value; }
    public float[] renderMeshData { get => _renderMeshData; set => _renderMeshData = value; }
    public int vbo { get => _vbo; set => _vbo = value; }
    public int vao { get => _vao; set => _vao = value; }
    public int ebo { get => _ebo; set => _ebo = value; }
}