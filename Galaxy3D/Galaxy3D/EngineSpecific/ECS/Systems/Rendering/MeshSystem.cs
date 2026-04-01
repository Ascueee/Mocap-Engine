using Galaxy3D.Assets;
using Galaxy3D.ECS.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Galaxy3D.ECS.Systems;

public class MeshSystem : ISystem
{
    private Entity[] _renderEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;
    private Entity _cameraEntity;
    //Lights
    private List<Entity> _lights = new List<Entity>(); //TODO CHANGE: THIS TO A LIST OF LIGHTS COMPONENTS
    

    public MeshSystem(int MAX_ENTITIES)
    {
        _renderEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }
    
    public void LoadSystem()
    {
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            //Console.WriteLine(_renderEntities[i].entityName);

            //Add lights to system
            if (_renderEntities[i].HasComponent<DirectionalLight>())
            {
                Console.WriteLine(_renderEntities[i].entityName + " Light Added");
                _lights.Add(_renderEntities[i]);
                Console.WriteLine("Number of Lights in system: " + _lights.Count);
            }
            
            
            if (!_renderEntities[i].HasComponent<MeshRenderer>()) continue;
            Material entityMat = _renderEntities[i].GetComponent<Material>();
            MeshRenderer entityMesh = _renderEntities[i].GetComponent<MeshRenderer>();
                
            //Load the shader
            entityMat.shader.Load();

            if (entityMat.texture is not null)
            {
                entityMat.texture.Load();
                if (entityMat.texture is TextureAtlas)
                {
                    TextureAtlas atlas;
                    atlas = (TextureAtlas)entityMat.texture;
                    
                    entityMesh.uvData = atlas.UpdateMeshUVs(entityMat.atlasID, entityMesh.uvData);
                }
            }
                
            entityMesh.renderMeshData = CombineMeshData(entityMesh.meshData, entityMesh.uvData, entityMesh.normalData);
                
            entityMesh.vao = GL.GenVertexArray();
            GL.BindVertexArray(entityMesh.vao);
                
            entityMesh.vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, entityMesh.vbo);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                entityMesh.renderMeshData.Length * sizeof(float),
                entityMesh.renderMeshData,
                BufferUsageHint.StaticDraw
            );
                
            entityMesh.ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, entityMesh.ebo);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                entityMesh.indices.Length * sizeof(uint),
                entityMesh.indices,
                BufferUsageHint.StaticDraw
            );
                
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float,
                false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float),
                3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float),
                5 * sizeof(float));
            GL.EnableVertexAttribArray(2);
                
            if(entityMat.texture is not null)
                entityMat.texture.Use(TextureUnit.Texture0);
            _renderEntities[i].SetComponent<MeshRenderer>(entityMesh);
        }
    }
    
    public void UpdateSystem()
    {
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            int indexDir = 0;
            int indexPoint = 0;
            if (!_renderEntities[i].HasComponent<MeshRenderer>()) continue;
            
            Material entityMat = _renderEntities[i].GetComponent<Material>();
            MeshRenderer entityMesh = _renderEntities[i].GetComponent<MeshRenderer>();
            Transform entityTransform = _renderEntities[i].GetComponent<Transform>();
            Camera entityCamera = _cameraEntity.GetComponent<Camera>();
            Matrix4 model = entityTransform.modelMatrix;
            
            if (entityMat.texture is not null)
            {
                entityMat.texture.Use(TextureUnit.Texture0);  
                entityMat.shader.SetInt("mat.texture0", 0);
            }
            
            if (entityMat.texture is TextureAtlas && entityMat.previousID != entityMat.atlasID)
            {
                Console.WriteLine("Is this being run every frame");
                TextureAtlas atlas;
                atlas = (TextureAtlas)entityMat.texture;
                    
                entityMesh.uvData = atlas.UpdateMeshUVs(entityMat.atlasID, entityMesh.originalUVData);
                entityMesh.renderMeshData = CombineMeshData(entityMesh.meshData, entityMesh.uvData, entityMesh.normalData);
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, entityMesh.vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, entityMesh.renderMeshData.Length * sizeof(float),
                    entityMesh.renderMeshData, BufferUsageHint.StaticDraw);
                
                entityMat.previousID = entityMat.atlasID;
                _renderEntities[i].SetComponent(entityMesh);
                _renderEntities[i].SetComponent(entityMat);
            }
            
            entityMat.shader.Use();
            GL.BindVertexArray(entityMesh.vao);
            entityMat.shader.SetMat4("model", model);
            entityMat.shader.SetMat4("view", entityCamera.viewMatrix);
            entityMat.shader.SetMat4("projection", entityCamera.projectionMatrix);
            
            //Material Struct
            entityMat.shader.SetVec3("mat.color", entityMat.materialColor.Xyz);
            entityMat.shader.SetVec3("mat.specular", entityMat.specularStrenght);
            entityMat.shader.SetFloat("mat.shine", entityMat.shine);
            
            //Light Struct for dir light
            for (int l = 0; l < _lights.Count; l++)
            {
                DirectionalLight systemLight = _lights[l].GetComponent<DirectionalLight>();
                Transform lightTransform = _lights[l].GetComponent<Transform>();
                
                entityMat.shader.SetVec3($"dirLight[{l}].color", systemLight.lightColor);
                entityMat.shader.SetVec3($"dirLight[{l}].dir", lightTransform.position);
                entityMat.shader.SetVec3($"dirLight[{l}].ambient", systemLight.ambient);
                entityMat.shader.SetVec3($"dirLight[{l}].diffuse", systemLight.diffuse);
                entityMat.shader.SetVec3($"dirLight[{l}].specular", systemLight.specular);
            }
            

            GL.DrawElements(PrimitiveType.Triangles, entityMesh.indices.Length,
                DrawElementsType.UnsignedInt, 0);
        }
    }
    
    public void UpdateMesh(Entity e)
    {
        MeshRenderer mesh = e.GetComponent<MeshRenderer>();

        mesh.renderMeshData = CombineMeshData(mesh.meshData, mesh.uvData, mesh.normalData);

        GL.BindVertexArray(mesh.vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.vbo);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            mesh.renderMeshData.Length * sizeof(float),
            mesh.renderMeshData,
            BufferUsageHint.StaticDraw
        );

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.ebo);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            mesh.indices.Length * sizeof(uint),
            mesh.indices,
            BufferUsageHint.StaticDraw
        );

        e.SetComponent(mesh);
    }
    
    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _renderEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _renderEntities.CopyTo(updatedArray, 0);
            _renderEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _renderEntities[id] = e;
        _currentSystemAmount++;
    }
    public float[] CombineMeshData(float[] vertexData, float[] uvData, float[] normalData)
    {
        float[] combinedMesh = new float[vertexData.Length + uvData.Length + normalData.Length];
        
        int indexCounterMesh = 0;
        int indexCounterVertex = 0;
        int indexCounterUV = 0;
        int indexCounterNormal = 0;
        
        for (int i = 0; i < (combinedMesh.Length / 8); i++)
        {

            //these make up the vertices
            combinedMesh[indexCounterMesh] = vertexData[indexCounterVertex];
            combinedMesh[indexCounterMesh + 1] = vertexData[indexCounterVertex + 1];
            combinedMesh[indexCounterMesh + 2] = vertexData[indexCounterVertex + 2];
            
            //texture uvs
            combinedMesh[indexCounterMesh + 3] = uvData[indexCounterUV];
            combinedMesh[indexCounterMesh + 4] = uvData[indexCounterUV + 1];
            
            //normals
            combinedMesh[indexCounterMesh + 5] = normalData[indexCounterNormal];
            combinedMesh[indexCounterMesh + 6] = normalData[indexCounterNormal + 1];
            combinedMesh[indexCounterMesh + 7] = normalData[indexCounterNormal + 2];
            
            
            indexCounterMesh += 8;
            indexCounterVertex += 3;
            indexCounterUV += 2;
            indexCounterNormal += 3;
        }
        
        return combinedMesh;
    }
    
    public Entity camera { get => _cameraEntity; set => _cameraEntity = value; }
}