
using Assimp;
using Galaxy3D.ECS.Components;
using OpenTK.Mathematics;
using Soul.ECS.Components;
using Material = Soul.ECS.Components.Material;

namespace Galaxy3D.Assets.Models;

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
    
    /// 
    /// THIS FILE WILL BE EXPANED TO ADD:
    /// TEXTURES FROM MODELS
    /// MATERIALS FROM MODELS
    /// BLEND SHAPES/MORPH TARGETS
    /// SKELETON RIGGING INFORMATION
    /// </summary>

    List<Mesh> modelMeshes = new List<Mesh>();
    public Model(string modelFilePath, ECSWorld world)
    {
        var importer = new AssimpContext();
        var assimpScene = importer.ImportFile(modelFilePath, PostProcessSteps.Triangulate
                                                       | PostProcessSteps.GenerateSmoothNormals
                                                       | PostProcessSteps.FlipUVs
                                                       | PostProcessSteps.JoinIdenticalVertices
                                                       | PostProcessSteps.CalculateTangentSpace);
        
        foreach (Assimp.Mesh mesh in assimpScene.Meshes)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var v in mesh.Vertices)
            {
                min = Vector3.ComponentMin(min, new Vector3(v.X, v.Y, v.Z));
                max = Vector3.ComponentMax(max, new Vector3(v.X, v.Y, v.Z));
            }

            Vector3 center = (min + max) / 2f;
            
            float[] vertices = new float[mesh.Vertices.Count * 3];
            int vertexArrayIndex = 0;

            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                vertices[vertexArrayIndex]     = mesh.Vertices[i].X - center.X;
                vertices[vertexArrayIndex + 1] = mesh.Vertices[i].Y - center.Y;
                vertices[vertexArrayIndex + 2] = mesh.Vertices[i].Z - center.Z;

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
            TextureAtlas atlas = new TextureAtlas(
                "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/container2.png",
                500,
                new Vector2(500,500)
            );
            Shader testShader = new Shader(
                "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/Base.vert",
                "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/Base.frag");
            
            world.CreateEntity(mesh.Name);
            Mesh engineModelmesh = new Mesh(vertices, textureUVs, indices.ToArray());
            world.GetEntity(mesh.Name).AddComponent(new MeshRenderer(engineModelmesh));

            world.GetEntity(mesh.Name).AddComponent(new Material(
                testShader, 
                atlas, 
                new Vector4(1.0f, 0.5f, 0.5f, 1.0f))
            );
            world.GetEntity(mesh.Name).AddComponent(new Transform());
            
            Transform objectTransform = world.GetEntity(mesh.Name).GetComponent<Transform>();
            objectTransform.position = new Vector3(0f, 0f, 0f);
            objectTransform.scale = new Vector3(0.5f, 0.5f, 0.5f);
            //objectTransform.rotation += new Vector3(0.5f * (float)e.Time, 0f, 0f);

            world.GetEntity(mesh.Name).SetComponent(objectTransform);
        }
    }
}