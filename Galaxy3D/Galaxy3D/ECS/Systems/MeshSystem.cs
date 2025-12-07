using Galaxy3D.Assets;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Systems.ComponentSystems;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Soul.ECS.Components;

namespace Galaxy3D;

/// <summary>
///     Handles the logic behind the mesh renderer
/// </summary>
public class MeshSystem : ISystem
{
    private readonly Entity[] _entityMeshes;
    private readonly IdGenerator _idGenerator;
    private int currentSystemAmount;

    public MeshSystem(int MAX_ENTITIES)
    {
        _entityMeshes = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }

    //This is where all the data is loaded for rendering
    //Set up VAOs, and VBOS for each of the entities

    public void LoadSystem()
    {
        for (var i = 0; i < currentSystemAmount; i++)
        {
            Console.WriteLine(_entityMeshes[i].entityName);
            Material entityMat = _entityMeshes[i].GetComponent<Material>();
            MeshRenderer entityMesh = _entityMeshes[i].GetComponent<MeshRenderer>();

            //Load the shader
            entityMat.shader.Load(); 
            entityMat.texture.Load();

            if (entityMat.texture is TextureAtlas)
            {
                TextureAtlas atlas;
                atlas = (TextureAtlas)entityMat.texture;
                entityMesh.meshData = atlas.UpdateMeshUVs(0, entityMesh.meshData);
            }
            
            entityMesh.vao = GL.GenVertexArray();
            GL.BindVertexArray(entityMesh.vao);

            entityMesh.vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, entityMesh.vbo);
            GL.BufferData(
                BufferTarget.ArrayBuffer, entityMesh.meshData.Length * sizeof(float),
                entityMesh.meshData,
                BufferUsageHint.StaticDraw
            );

            //Now giving context to the vertex data
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
                false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            entityMat.texture.Use(TextureUnit.Texture0);
            _entityMeshes[i].SetComponent<MeshRenderer>(entityMesh);
        }
    }

    //Draw the mesh and update system logic for each rendering
    public void UpdateSystem()
    {
        
        for (var i = 0; i < currentSystemAmount; i++)
        {
            Material entityMat = _entityMeshes[i].GetComponent<Material>();
            MeshRenderer entityMesh = _entityMeshes[i].GetComponent<MeshRenderer>();

            Matrix4 model = Matrix4.CreateRotationX(45f);
            Matrix4 view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f),
                800f / 600f, 0.1f, 100.0f);

            entityMat.texture.Use(TextureUnit.Texture0);
            entityMat.shader.Use();
            
            GL.BindVertexArray(entityMesh.vao);
            entityMat.shader.SetVec4("color", entityMat.materialColor);
            entityMat.shader.SetMat4("model", model);
            entityMat.shader.SetMat4("view", view);
            entityMat.shader.SetMat4("projection", projection);
            GL.DrawArrays(PrimitiveType.Triangles, 0, entityMesh.meshData.Length / 5);
        }
    }

    public void AddEntityToSystem(Entity e)
    {
        var id = _idGenerator.Dequeue();
        _entityMeshes[id] = e;
        currentSystemAmount++;
    }
}