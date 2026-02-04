namespace Galaxy3D.Assets;
public struct Mesh
{
    const int _engineDataLength = 8; //represents the amount of data one Vertex has(vertex, uvs, normals, etc)
    float[] _vertices; //3
    float[] _uvs; //2
    float[] _normals; //3
    uint[] _indices;

    public Mesh(float[] meshData)
    {
        //initializes the arrays and sets their sizes
        _vertices = new float[(meshData.Length / _engineDataLength) * 3];
        _uvs = new float[(meshData.Length / _engineDataLength) * 2];
        _normals = new float[(meshData.Length / _engineDataLength) * 3];
        GenerateMeshData(meshData);
    }
    
    public Mesh(float[] meshData, uint[] indices)
    {
        //initializes the arrays and sets their sizes
        _vertices = new float[(meshData.Length / _engineDataLength) * 3];
        _uvs = new float[(meshData.Length / _engineDataLength) * 2];
        _normals = new float[(meshData.Length / _engineDataLength) * 3];
        _indices = indices;
 
        
        GenerateMeshData(meshData);
    }

    //Load a mesh for a model file with seperate arrays for each data
    public Mesh(float[] vertices, float[] uvs, float[] normals ,uint[] indices)
    {
        _vertices = vertices;
        _uvs = uvs;
        _indices = indices;
        _normals = normals;
    }
    
    /// This organizes the meshes data into its seperate data channels(vertices, uvs, later normals, etc)
    public void GenerateMeshData(float[] meshData)
    {
        int indexCounterMesh = 0;
        int indexCounterVertex = 0;
        int indexCounterUV = 0;
        int indexCounterNormal = 0;
        
        for (int i = 0; i < (meshData.Length / _engineDataLength); i++)
        {
            _vertices[indexCounterVertex] = meshData[indexCounterMesh];
            _vertices[indexCounterVertex + 1] = meshData[indexCounterMesh + 1];
            _vertices[indexCounterVertex + 2] = meshData[indexCounterMesh + 2];
            
            _uvs[indexCounterUV] = meshData[indexCounterMesh + 3];
            _uvs[indexCounterUV + 1] = meshData[indexCounterMesh + 4];
            
            _normals[indexCounterNormal] = meshData[indexCounterMesh + 5];
            _normals[indexCounterNormal + 1] = meshData[indexCounterMesh + 6];
            _normals[indexCounterNormal + 2] = meshData[indexCounterMesh + 7];
            
            indexCounterMesh += _engineDataLength;
            indexCounterVertex += 3;
            indexCounterUV += 2;
            indexCounterNormal += 3;
        }
    }
    
    public float[] vertices { get => _vertices; set => _vertices = value; }
    public float[] uvs { get => _uvs; set => _uvs = value; }
    public float[] normals { get => _normals; set => _normals = value; }
    public uint[] indices { get => _indices; set => _indices = value; }

}