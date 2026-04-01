namespace Galaxy3D.Assets;

public struct Mesh
{
    const int _engineDataLength = 8; //represents the amount of data one Vertex has(vertex, uvs, normals, etc)
    private string _meshName;
    float[] _vertices; //3
    float[] _uvs; //2
    float[] _normals; //3
    uint[] _indices;
    private float[] _originalUvs;
    
    public Mesh(float[] meshData)
    {
        //initializes the arrays and sets their sizes
        _vertices = new float[(meshData.Length / _engineDataLength) * 3];
        _uvs = new float[(meshData.Length / _engineDataLength) * 2];
        _originalUvs = new float[(meshData.Length / _engineDataLength) * 2];
        _normals = new float[(meshData.Length / _engineDataLength) * 3];
        GenerateMeshData(meshData);
    }
    
    public Mesh(float[] meshData, uint[] indices)
    {
        //initializes the arrays and sets their sizes
        _vertices = new float[(meshData.Length / _engineDataLength) * 3];
        _uvs = new float[(meshData.Length / _engineDataLength) * 2];
        _originalUvs = new float[(meshData.Length / _engineDataLength) * 2];
        _normals = new float[(meshData.Length / _engineDataLength) * 3];
        _indices = indices;
 
        
        GenerateMeshData(meshData);
    }

    //Load a mesh for a model file with seperate arrays for each data
    public Mesh(float[] vertices, float[] uvs, float[] normals ,uint[] indices)
    {
        _vertices = vertices;
        _uvs = uvs;
        _originalUvs = uvs;
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
            
            _originalUvs[indexCounterUV] = meshData[indexCounterMesh + 3];
            _originalUvs[indexCounterUV + 1] = meshData[indexCounterMesh + 4];
            
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
    public float[] originalUVs { get => _originalUvs; set => _originalUvs = value; }
    public float[] normals { get => _normals; set => _normals = value; }
    public uint[] indices { get => _indices; set => _indices = value; }
    
    public static Mesh cubeMesh  { get => new Mesh(
        new float[]
        {
            // Back (-Z)
            -0.5f, -0.5f, -0.5f, 0f, 0f,  0f, 0f, -1f,
            0.5f, -0.5f, -0.5f, 1f, 0f,  0f, 0f, -1f,
            0.5f,  0.5f, -0.5f, 1f, 1f,  0f, 0f, -1f,
            -0.5f,  0.5f, -0.5f, 0f, 1f,  0f, 0f, -1f,
        
            // Front (+Z)
            -0.5f, -0.5f, 0.5f, 0f, 0f,   0f, 0f, 1f,
            0.5f, -0.5f, 0.5f, 1f, 0f,   0f, 0f, 1f,
            0.5f,  0.5f, 0.5f, 1f, 1f,   0f, 0f, 1f,
            -0.5f,  0.5f, 0.5f, 0f, 1f,   0f, 0f, 1f,

            // Left (-X)
            -0.5f, -0.5f, -0.5f, 0f, 0f,  -1f, 0f, 0f,
            -0.5f, -0.5f,  0.5f, 1f, 0f,  -1f, 0f, 0f,
            -0.5f,  0.5f,  0.5f, 1f, 1f,  -1f, 0f, 0f,
            -0.5f,  0.5f, -0.5f, 0f, 1f,  -1f, 0f, 0f,
        
            // Right (+X)
            0.5f, -0.5f, -0.5f, 0f, 0f,   1f, 0f, 0f,
            0.5f, -0.5f,  0.5f, 1f, 0f,   1f, 0f, 0f,
            0.5f,  0.5f,  0.5f, 1f, 1f,   1f, 0f, 0f,
            0.5f,  0.5f, -0.5f, 0f, 1f,   1f, 0f, 0f,
        
            // Bottom (-Y)
            -0.5f, -0.5f, -0.5f, 0f, 0f,   0f, -1f, 0f,
            0.5f, -0.5f, -0.5f, 1f, 0f,   0f, -1f, 0f,
            0.5f, -0.5f,  0.5f, 1f, 1f,   0f, -1f, 0f,
            -0.5f, -0.5f,  0.5f, 0f, 1f,   0f, -1f, 0f,

            // Top (+Y)
            -0.5f,  0.5f, -0.5f, 0f, 0f,   0f, 1f, 0f,
            0.5f,  0.5f, -0.5f, 1f, 0f,   0f, 1f, 0f,
            0.5f,  0.5f,  0.5f, 1f, 1f,   0f, 1f, 0f,
            -0.5f,  0.5f,  0.5f, 0f, 1f,   0f, 1f, 0f,
        }, 
        new uint[]
        {
            0, 3, 2, 2, 1, 0,    // Back 
            4, 5, 6, 6, 7, 4,    // Front
            8, 9, 10, 10, 11, 8, // Left
            12, 15, 14, 14, 13, 12, // Right 
            16, 17, 18, 18, 19, 16, // Bottom
            20, 23, 22, 22, 21, 20  // Top
        });
    }
    public static Mesh squareMesh { get => new Mesh(
        new float[] {
            0.5f,  0.5f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,  // top right
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f // bottom left
                -0.5f,  0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f // top left 
        }, 
        new uint[]
        {
            0, 1, 3, 
            1, 2, 3  
        });
    }
    public static Mesh triangleMesh {get => new Mesh(
        new float[]
        {
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // Bottom-left vertex
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // Bottom-right vertex
            0.0f,  0.5f, 0.0f, 0.5f, 1.0f  // Top vertex
        }, 
        new uint[]
        {
            0,1,2
        });
    }

}