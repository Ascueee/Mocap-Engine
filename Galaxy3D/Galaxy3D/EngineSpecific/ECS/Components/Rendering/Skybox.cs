namespace Galaxy3D.ECS.Components;

public struct Skybox : IComponent
{
    private int _vao;
    private int _vbo;
    private static readonly float[] Vertices = {
        -1, -1,  1,   1, -1,  1,   1,  1,  1,   1,  1,  1,  -1,  1,  1,  -1, -1,  1,
        -1, -1, -1,  -1,  1, -1,   1,  1, -1,   1,  1, -1,   1, -1, -1,  -1, -1, -1,
        -1,  1, -1,  -1,  1,  1,   1,  1,  1,   1,  1,  1,   1,  1, -1,  -1,  1, -1,
        -1, -1, -1,   1, -1, -1,   1, -1,  1,   1, -1,  1,  -1, -1,  1,  -1, -1, -1,
        1, -1, -1,   1,  1, -1,   1,  1,  1,   1,  1,  1,   1, -1,  1,   1, -1, -1,
        -1, -1, -1,  -1, -1,  1,  -1,  1,  1,  -1,  1,  1,  -1,  1, -1,  -1, -1, -1
    };

    public Skybox() { }

    public int componentID { get; set; }
    public bool isActive { get; set; }
    public int vao { get => _vao; set => _vao = value; }
    public int vbo { get => _vbo; set => _vbo = value; }
    public float[] vertices => Vertices;
    
    
}