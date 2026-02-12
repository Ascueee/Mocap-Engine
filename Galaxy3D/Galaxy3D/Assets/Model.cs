using Assimp;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using OpenTK.Mathematics;
using Material = Galaxy3D.ECS.Components.Material;

namespace Galaxy3D.Assets;

public class Model
{
    /// <summary>
    /// MODEL LOADING DEV NOTES:
    /// NEED TO COME BACK AND UPDATE THIS TO WORK WITH THE ENGINE HIERACHY(not implemented)
    /// Need to create new entities for each mesh that is in the asssimp scene
    ///     The first entity will be a general entity to house all the assimp meshes
    ///     Then the children of the base entity will have children that represent the meshes and 
    ///     also children rigged objects
    ///     The bones need to retrieved for the animation system later
    ///     Also need to create a skinned renderer for skeletal animation
    /// Then need to convert the assimp mesh data to work with my engines Mesh asset
    /// The Mesh asset will then be used in the Mesh renderer to render the model
    /// THIS FILE WILL BE EXPANED TO ADD:
    /// TEXTURES FROM MODELS
    /// MATERIALS FROM MODELS
    /// BLEND SHAPES/MORPH TARGETS
    /// SKELETON RIGGING INFORMATION
    /// </summary>

    List<Mesh> modelMeshes = new List<Mesh>();
    private Entity _modelMainEntity;
    
    public Model(string modelName, string modelFilePath, ECSWorld world)
    {
        var importer = new AssimpContext();
        var assimpScene = importer.ImportFile(modelFilePath, PostProcessSteps.Triangulate
                                                       | PostProcessSteps.GenerateSmoothNormals
                                                       | PostProcessSteps.FlipUVs
                                                       | PostProcessSteps.JoinIdenticalVertices
                                                       | PostProcessSteps.CalculateTangentSpace);
        
        world.CreateEntity(modelName);
        _modelMainEntity = world.GetEntity(modelName);
        _modelMainEntity.AddComponent(new Transform());
        
        TextureAtlas atlas = new TextureAtlas(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/container2.png",
            1,
            new Vector2(1,1)
        );
        Shader testShader = new Shader(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.vert",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.frag");
        
        //Itterates through the model mesh can creates entities
        foreach (Assimp.Mesh mesh in assimpScene.Meshes)
        {
            float[] vertices = new float[mesh.Vertices.Count * 3];
            int vertexArrayIndex = 0;

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                vertices[vertexArrayIndex] = mesh.Vertices[i].X;
                vertices[vertexArrayIndex + 1] = mesh.Vertices[i].Y;
                vertices[vertexArrayIndex + 2] = mesh.Vertices[i].Z;

                vertexArrayIndex += 3;
            }
            
            int vertexCount = mesh.VertexCount;
            float[] textureUVs = new float[vertexCount * 2];

            if (mesh.HasTextureCoords(0))
            {
                for (int i = 0; i < vertexCount; i++)
                {
                    var uv = mesh.TextureCoordinateChannels[0][i];
                    textureUVs[i * 2 + 0] = uv.X;
                    textureUVs[i * 2 + 1] = uv.Y;
                }
            }
            else
            {
                Console.WriteLine("Mesh has no UVs");
            }
            
            float[] normals = new float[mesh.Normals.Count * 3];
            if (mesh.HasNormals == true)
            {
                vertexArrayIndex = 0;
                for (int i = 0; i < mesh.Normals.Count; i++)
                {
                    normals[vertexArrayIndex] = mesh.Normals[i].X;
                    normals[vertexArrayIndex + 1] = mesh.Normals[i].Y;
                    normals[vertexArrayIndex + 2] = mesh.Normals[i].Z;

                    vertexArrayIndex += 3;
                }
            }
            
            List<uint> indices = new List<uint>();
            foreach (var face in mesh.Faces)
            {
                if (face.IndexCount == 3)
                {
                    indices.Add((uint)face.Indices[0]);
                    indices.Add((uint)face.Indices[1]);
                    indices.Add((uint)face.Indices[2]);
                }
            }

            world.CreateEntity(mesh.Name);
            Console.WriteLine(mesh.Name);
            _modelMainEntity.children.Add(mesh.Name, world.GetEntity(mesh.Name));
            Mesh engineModelmesh = new Mesh(vertices, textureUVs, normals ,indices.ToArray());
            world.GetEntity(mesh.Name).AddComponent(new MeshRenderer(engineModelmesh));
            world.GetEntity(mesh.Name).AddComponent(new Material(
                testShader, 
                atlas, 
                new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                0.2f,
                32f)
            );
            world.GetEntity(mesh.Name).AddComponent(new Transform());
            
            //Updates the transform off the object
            Transform objectTransform = world.GetEntity(mesh.Name).GetComponent<Transform>();
            objectTransform.position = new Vector3(0f, -5f, 0f);
            objectTransform.scale = new Vector3(0.01f, 0.01f, 0.01f);
            
            world.GetEntity(mesh.Name).SetComponent(objectTransform);
        }
    }
}