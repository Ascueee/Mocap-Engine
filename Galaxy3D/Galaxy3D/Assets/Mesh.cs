namespace Galaxy3D.Assets;
public struct Mesh
{
    float[] _vertices;

    public Mesh(float[] vertices)
    {
        _vertices = vertices;
    }
    
    public float[] vertices { get => _vertices; set => _vertices = value; }

}