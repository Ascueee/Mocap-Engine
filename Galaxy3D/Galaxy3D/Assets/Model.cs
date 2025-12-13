
using Assimp;
using Soul.ECS.Components;

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

    public Model(string modelFilePath, ECSWorld world)
    {
        var importer = new AssimpContext();
        var assimpScene = importer.ImportFile(modelFilePath, PostProcessSteps.Triangulate
                                                       | PostProcessSteps.GenerateSmoothNormals
                                                       | PostProcessSteps.FlipUVs
                                                       | PostProcessSteps.JoinIdenticalVertices
                                                       | PostProcessSteps.CalculateTangentSpace);
        //Need to create a entity with my engine mesh implemtation for each assimp mesh
        foreach (Assimp.Mesh mesh in assimpScene.Meshes)
        {
          
            float[] vertices = new float[mesh.Vertices.Count * 3];
            int vertexArrayIndex = 0;
            //Now need to create a float[] to convert the mesh data because Assimp meshes hold their vertices in Vector3
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
            
            List<uint> indices = new List<uint>();
            
            foreach (var face in mesh.Faces)
            {
                // Safety check (Assimp can load non-triangle faces)
                if (face.IndexCount == 3)
                {
                    indices.Add((uint)face.Indices[0]);
                    indices.Add((uint)face.Indices[1]);
                    indices.Add((uint)face.Indices[2]);
                }
            }
            
            //first need to create entity the represents the mesh
            world.CreateEntity(mesh.Name);
            //this will be where the model stores its info to be rendered in the mesh system
            Mesh engineModelmesh = new Mesh(vertices, textureUVs, indices.ToArray());
            world.GetEntity(0).AddComponent(new MeshRenderer(engineModelmesh));
            
        }
    }
}