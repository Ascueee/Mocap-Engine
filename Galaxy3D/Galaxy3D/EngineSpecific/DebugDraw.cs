using Galaxy3D.Assets;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Components.Physics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Galaxy3D.EngineSpecific;

/// <summary>
/// Used to visualize componenets and debugging
/// </summary>
public class DebugDraw
{
    private int _vbo;
    private int _vao;
    private List<Vector3> _lines = new List<Vector3>();
    List<Entity> _AABBColliders = new List<Entity>();
    
    private Entity _camera;
    private Shader _debugShader = new Shader(
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/DebugShader.vert",
        "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/DebugShader.frag");
    
    public void Load()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);
        
        _debugShader.Load();
    }
    
    
    public void DrawAABB(AABB box)
    {
        Vector3 min = box.min;
        Vector3 max = box.max;

        Vector3[] c =
        {
            new(min.X,min.Y,min.Z),
            new(max.X,min.Y,min.Z),
            new(max.X,max.Y,min.Z),
            new(min.X,max.Y,min.Z),

            new(min.X,min.Y,max.Z),
            new(max.X,min.Y,max.Z),
            new(max.X,max.Y,max.Z),
            new(min.X,max.Y,max.Z)
        };

        int[] indices =
        {
            0,1, 1,2, 2,3, 3,0,
            4,5, 5,6, 6,7, 7,4,
            0,4, 1,5, 2,6, 3,7
        };

        for (int i = 0; i < indices.Length; i += 2)
        {
            _lines.Add(c[indices[i]]);
            _lines.Add(c[indices[i+1]]);
        }
    }

    public void PersistantDraw()
    {
        foreach (Entity e in _AABBColliders)
        {
            AABB box = e.GetComponent<AABB>();
            DrawAABB(box);
        }
    }
    
    public void DrawRay(Vector3 origin, Vector3 dir, float length)
    {
        Vector3 end = origin + dir * length;

        _lines.Add(origin);
        _lines.Add(end);
    }
    
    
    public void RenderLines()
    {
        if (_lines.Count == 0)
            return;
        
        Camera cam = _camera.GetComponent<Camera>();
        _debugShader.Use();
        _debugShader.SetMat4("view", cam.viewMatrix);
        _debugShader.SetMat4("projection", cam.projectionMatrix);
        _debugShader.SetVec3("color", new Vector3(1.0f, 0.0f, 1.0f));
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        GL.BufferData(
            BufferTarget.ArrayBuffer,
            _lines.Count * Vector3.SizeInBytes,
            _lines.ToArray(),
            BufferUsageHint.DynamicDraw
        );

        GL.DrawArrays(PrimitiveType.Lines, 0, _lines.Count);

        GL.BindVertexArray(0);

        _lines.Clear();
    }
    
    public Entity camera { get => _camera; set => _camera = value; }
    public List<Entity> AABBColliders { get => _AABBColliders; set => _AABBColliders = value; }
}