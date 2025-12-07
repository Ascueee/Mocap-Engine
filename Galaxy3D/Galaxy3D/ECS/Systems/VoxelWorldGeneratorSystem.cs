using Galaxy3D.ECS.Components;
using Galaxy3D.ECS.Systems.ComponentSystems;
using RenderEye.EngineSpecific;

namespace Galaxy3D.ECS.Systems;

public class VoxelWorldGeneratorSystem<T> : ISystem where T : Voxel, new()
{
    private Entity _entityWorldGenerator;
    
    public void LoadSystem()
    {
        GenerateVoxelWorld();
    }

    //Run system logic
    public void UpdateSystem()
    {
        
    }

    /// <summary>
    /// Generates an entire world using the world size
    /// </summary>
    public void GenerateVoxelWorld()
    {
        VoxelWorld<T> voxelWorld = _entityWorldGenerator.GetComponent<VoxelWorld<T>>();
        
        for (int x = 0; x < voxelWorld.worldChunks.GetLength(0); x++)
        {
            for (int y = 0; y < voxelWorld.worldChunks.GetLength(1); y++)
            {
                voxelWorld.worldChunks[x, y] = new VoxelChunk<T>(voxelWorld.chunkXZSize, voxelWorld.chunkYSize);
                voxelWorld.worldChunks[x, y].GenerateChunk(voxelWorld.voxelRegistry);
            }
        }
    }

    public void AddEntityToSystemw(Entity e)
    {
        _entityWorldGenerator = e;
    }
}