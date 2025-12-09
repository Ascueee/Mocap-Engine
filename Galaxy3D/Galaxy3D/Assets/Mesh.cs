namespace Galaxy3D.Assets;
public struct Mesh
{
    private const int _engineDataLength = 5; //represents the amount of data one Vertex has(vertex, uvs, normals, etc)
    float[] _vertices;
    private float[] _uvs;

    public Mesh(float[] meshData)
    {
        //initializes the arrays and sets their sizes
        _vertices = new float[(meshData.Length / _engineDataLength) * 3];
        _uvs = new float[(meshData.Length / _engineDataLength) * 2];
        
        GenerateMeshData(meshData);
    }


    /// This organizes the meshes data into its seperate data channels(vertices, uvs, later normals, etc)
    public void GenerateMeshData(float[] meshData)
    {
        int indexCounterMesh = 0;
        int indexCounterVertex = 0;
        int indexCounterUV = 0;
        
        
        for (int i = 0; i < (meshData.Length / _engineDataLength); i++)
        {
            _vertices[indexCounterVertex] = meshData[indexCounterMesh];
            _vertices[indexCounterVertex + 1] = meshData[indexCounterMesh + 1];
            _vertices[indexCounterVertex + 2] = meshData[indexCounterMesh + 2];
            
            _uvs[indexCounterUV] = meshData[indexCounterMesh + 3];
            _uvs[indexCounterUV + 1] = meshData[indexCounterMesh + 4];
            
            indexCounterMesh += _engineDataLength;
            indexCounterVertex += 3;
            indexCounterUV += 2;
        }
    }
    
    public float[] vertices { get => _vertices; set => _vertices = value; }
    public float[] uvs { get => _uvs; set => _uvs = value; }

}