
namespace Galaxy3D.EngineSpecific;

/// <summary>
/// This is a basic data contained that will hold data pertaining to what is being housed in the voxel grid
/// </summary>
public class Voxel
{
    public int _voxelID;

    public Voxel() { }

    public virtual void SetVoxelID(int id)
    {
        _voxelID = id;
    }
    
}