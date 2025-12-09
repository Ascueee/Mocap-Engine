namespace Galaxy3D.EngineSpecific;

/// <summary>
/// This will hold a list of registed voxels
/// Registered Voxels are like templates of a voxel type
/// the base voxel implementation is a container of data
/// The register allows for the container of data to be used with a specic contex
/// EX: Voxel interface could be a block
/// The Block Register is where a Dirt block and its properties be added
/// </summary>
public class VoxelRegistry<T> where T : Voxel, new()
{
    Dictionary<int, T> _registeredVoxels = new Dictionary<int, T>();
    
    public void RegisterVoxel(int voxelID, T voxel)
    {
        voxel.SetVoxelID(voxelID);
        _registeredVoxels.Add(voxelID, voxel);
    }

    public T GetVoxel(int voxelID)
    {
        return _registeredVoxels[voxelID];
    }
}