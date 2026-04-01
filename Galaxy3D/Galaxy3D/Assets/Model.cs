using System.Numerics;
using Assimp;
using Galaxy3D.ECS;
using Galaxy3D.ECS.Components;
using OpenTK.Mathematics;
using Material = Galaxy3D.ECS.Components.Material;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace Galaxy3D.Assets;

public class Model
{
    /// <summary>
    /// MODEL LOADING DEV NOTES:
    /// From last time: model materials are implemented as well as a child parent relationship being fully created for models
    /// Also modeles are now not gotten from the absolute path but from the assets folder directory makin it universal
    /// HOWEVER: need to add a asset manager for easy directory getting for models and other assets
    /// THIS FILE WILL BE EXPANED TO ADD:
    /// MESHES(DONE)
    /// TEXTURES FROM MODELS(DONE)
    /// MATERIALS FROM MODELS(DONE)
    /// NEED TO IMPLEMENT WITH MESH SYSTEM(DONE)
    /// BLEND SHAPES/MORPH TARGETS
    /// SKELETON RIGGING INFORMATION
    /// </summary>
    
    private Entity _modelMainEntity;


    public Model(string modelName, string fileExtenstion, ECSWorld world)
    { 

        //TODO: COME BACK AND ADD A ASSET MANAGER FOR EASY ASSET GATHERING
        //Right now creates an assets folder where the 
        string baseDirectory = Directory.GetCurrentDirectory() + $"/Assets/Models/{modelName}/";
        Console.WriteLine(baseDirectory + modelName);
  
        var importer = new AssimpContext();
        var assimpScene = importer.ImportFile(baseDirectory + modelName + fileExtenstion, 
            PostProcessSteps.Triangulate
            | PostProcessSteps.GenerateSmoothNormals
            | PostProcessSteps.FlipUVs
            | PostProcessSteps.JoinIdenticalVertices
            | PostProcessSteps.CalculateTangentSpace
            | PostProcessSteps.LimitBoneWeights
            | PostProcessSteps.GlobalScale);
        
        
        //Creates the base entity for the model
        world.CreateEntity(modelName);
        _modelMainEntity = world.GetEntity(modelName);
        _modelMainEntity.AddComponent(new Transform());
        LoadModel(assimpScene, baseDirectory ,world);
    }

    void LoadModel(Scene assimpScene, string baseDirectory, ECSWorld world)
    {
        Texture modelTexture = new Texture("/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/white.png");
        
        Shader testShader = new Shader(
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.vert",
            "/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Shaders/EngineBase.frag");

        // Walk the node tree instead of iterating meshes flat
        ProcessNode(assimpScene.RootNode, assimpScene, baseDirectory, world, testShader, _modelMainEntity);
    }

    void ProcessNode(Node node, Scene assimpScene, string baseDirectory, ECSWorld world, Shader testShader, Entity parentEntity)
    {
        Entity nodeEntity = parentEntity;

        if (node.MeshCount > 0 || node.ChildCount > 0)
        {
            string nodeEntityName = $"{_modelMainEntity.entityName}_{node.Name}";
            world.CreateEntity(nodeEntityName);
            nodeEntity = world.GetEntity(nodeEntityName);
            
            // Node/joint entities get the transform from FBX
            var m = node.Transform;
            OpenTK.Mathematics.Vector3 nodePos = new OpenTK.Mathematics.Vector3(m.M14, m.M24, m.M34);
            OpenTK.Mathematics.Vector3 nodeScale = new OpenTK.Mathematics.Vector3(
                new System.Numerics.Vector3(m.M11, m.M21, m.M31).Length(),
                new System.Numerics.Vector3(m.M12, m.M22, m.M32).Length(),
                new System.Numerics.Vector3(m.M13, m.M23, m.M33).Length()
            );
            
            Transform nodeTransform = new Transform();
            nodeTransform.position = nodePos;
            nodeTransform.scale = nodeScale;
            nodeTransform.isDirty = true;
            nodeEntity.AddComponent(nodeTransform);
            parentEntity.SetChild(nodeEntity);
        }

        int index = 0;
        foreach (int meshIndex in node.MeshIndices)
        {
            index++;
            Assimp.Mesh mesh = assimpScene.Meshes[meshIndex];
            Assimp.Material material = assimpScene.Materials[mesh.MaterialIndex];

            float[] vertices = new float[mesh.Vertices.Count * 3];
            int vertexArrayIndex = 0;
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                vertices[vertexArrayIndex]     = mesh.Vertices[i].X;
                vertices[vertexArrayIndex + 1] = mesh.Vertices[i].Y;
                vertices[vertexArrayIndex + 2] = mesh.Vertices[i].Z;
                vertexArrayIndex += 3;
            }

            int textureUvCount = mesh.VertexCount;
            float[] textureUVs = new float[textureUvCount * 2];
            if (mesh.HasTextureCoords(0))
            {
                for (int i = 0; i < textureUvCount; i++)
                {
                    var uv = mesh.TextureCoordinateChannels[0][i];
                    textureUVs[i * 2]     = uv.X;
                    textureUVs[i * 2 + 1] = uv.Y;
                }
            }
            else { Console.WriteLine("Mesh has no UVs"); }

            Texture modelTexture = new Texture("/Users/hayyan/Desktop/Repos/Mocap-Engine/Galaxy3D/Galaxy3D/Assets/Textures/white.png");
            if (material.HasTextureDiffuse)
            {
                string texPath = Path.Combine(baseDirectory, material.TextureDiffuse.FilePath.Replace("\\", "/"));
                if (File.Exists(texPath))
                    modelTexture = new Texture(texPath);
                else
                    Console.WriteLine($"Warning: Texture {texPath} not found, using default.");
            }

            float[] normals = new float[mesh.Normals.Count * 3];
            if (mesh.HasNormals)
            {
                vertexArrayIndex = 0;
                for (int i = 0; i < mesh.Normals.Count; i++)
                {
                    normals[vertexArrayIndex]     = mesh.Normals[i].X;
                    normals[vertexArrayIndex + 1] = mesh.Normals[i].Y;
                    normals[vertexArrayIndex + 2] = mesh.Normals[i].Z;
                    vertexArrayIndex += 3;
                }
            }

            List<uint> indices = new List<uint>();
            foreach (var face in mesh.Faces)
                if (face.IndexCount == 3)
                {
                    indices.Add((uint)face.Indices[0]);
                    indices.Add((uint)face.Indices[1]);
                    indices.Add((uint)face.Indices[2]);
                }

            string meshEntityName = $"{_modelMainEntity.entityName}_mesh_{mesh.Name}_{index}";
            world.CreateEntity(meshEntityName);
            world.GetEntity(meshEntityName).AddComponent(new MeshRenderer(new Mesh(vertices, textureUVs, normals, indices.ToArray())));
            world.GetEntity(meshEntityName).AddComponent(new Material(
                testShader,
                modelTexture,
                new Vector4(material.ColorDiffuse.X, material.ColorDiffuse.Y,
                            material.ColorDiffuse.Z, material.ColorDiffuse.W),
                0.2f,
                1f));
            
            // Mesh entity gets identity transform — vertices are already in local node space
            world.GetEntity(meshEntityName).AddComponent(new Transform());
            nodeEntity.SetChild(world.GetEntity(meshEntityName));
        }

        foreach (Node child in node.Children)
            ProcessNode(child, assimpScene, baseDirectory, world, testShader, nodeEntity);
    }
    
}