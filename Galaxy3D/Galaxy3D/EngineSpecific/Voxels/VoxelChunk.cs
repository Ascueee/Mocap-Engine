using Galaxy3D.Assets;

namespace Galaxy3D.EngineSpecific.Voxels;

/// <summary>
/// Constructor takes in the size of the chunk its one int because the chunks are square so 5x5x5
/// </summary>
public class VoxelChunk<T> where T : Voxel, new()
{
    private T[,,] _voxels;
    private Mesh chunkMesh = new Mesh();
    public VoxelChunk(int chunkSizeXZ, int chunkSizeY)
    {
        _voxels = new T[chunkSizeXZ, chunkSizeY, chunkSizeXZ];
    }

    
    //Fill the chunks with voxels and have it place specific voxels with a palette
    //The chunk will be filled out but using the coordinates and placing a voxel using and ID
    //The voxelID connects to a enum called blockPallete that each index will represent the voxelID(t
    public void GenerateChunk(VoxelRegistry<T> voxelRegistry)
    {
        for (int x = 0; x < _voxels.GetLength(0); x++)
        {
            for (int y = 0; y < _voxels.GetLength(1); y++)
            {
                for (int z = 0; z < _voxels.GetLength(2); z++)
                {
                    AddVoxel(x, y, z, voxelRegistry.GetVoxel(0));
                }
            }
        }
    }
    
    void AddVoxel(int x, int y, int z, T voxel) 
    { 
        voxel.SetVoxelID(x);
        _voxels[x, y, z] = voxel;
    }
    
}