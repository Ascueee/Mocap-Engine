using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Systems;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Galaxy3D.EngineSpecific.ECS.Systems.Rendering;

public class SkyboxSystem : ISystem
{
    private Entity[] _skyboxEntities;
    private readonly IdGenerator _idGenerator;
    private int _currentSystemAmount;
    int _maxEntities = 100;
    private Entity _cameraEntity;
    
    public SkyboxSystem(int MAX_ENTITIES)
    {
        _skyboxEntities = new Entity[MAX_ENTITIES];
        _idGenerator = new IdGenerator(MAX_ENTITIES);
    }

    public void LoadSystem()
    {
        GL.Enable(EnableCap.TextureCubeMapSeamless);
        
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            Skybox skybox = _skyboxEntities[i].GetComponent<Skybox>();
            Material entityMat = _skyboxEntities[i].GetComponent<Material>();
            skybox.vao = GL.GenVertexArray();
            skybox.vbo = GL.GenBuffer();
            _skyboxEntities[i].SetComponent(skybox);
            entityMat.texture.Load();
            entityMat.shader.Load();
            
            GL.BindVertexArray(skybox.vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, skybox.vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, skybox.vertices.Length * sizeof(float),
                skybox.vertices, BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }

    public void UpdateSystem()
    {
        GL.Disable(EnableCap.CullFace); 
        GL.DepthMask(false);
        GL.DepthFunc(DepthFunction.Lequal);
        for (int i = 0; i < _currentSystemAmount; i++)
        {
            Skybox skybox = _skyboxEntities[i].GetComponent<Skybox>();
            Material entityMat = _skyboxEntities[i].GetComponent<Material>();
            Camera entityCamera = _cameraEntity.GetComponent<Camera>();

            entityMat.shader.Use();

            Matrix4 view = new Matrix4(new Matrix3(entityCamera.viewMatrix));
            entityMat.texture.Use(TextureUnit.Texture0);

            entityMat.shader.SetMat4("view", view);
            entityMat.shader.SetMat4("projection", entityCamera.projectionMatrix);
            entityMat.shader.SetInt("skybox", 0);
            

            GL.BindVertexArray(skybox.vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.BindVertexArray(0);
        }
        GL.DepthMask(true);
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.CullFace);
    }
    
    public void AddEntityToSystem(Entity e)
    {
        if (_currentSystemAmount == _skyboxEntities.Length)
        {
            _maxEntities += 100;
            _idGenerator.maxAmountOfIds = _maxEntities;
            var updatedArray = new Entity[_maxEntities];
            _skyboxEntities.CopyTo(updatedArray, 0);
            _skyboxEntities = updatedArray;
        }
        
        var id = _idGenerator.CreateEntityID();
        _skyboxEntities[id] = e;
        _currentSystemAmount++;
    }
    
   public Entity camera { get => _cameraEntity; set => _cameraEntity = value; }
}