using Galaxy3D.EngineSpecific;
using Galaxy3D.EngineSpecific.Voxels;

namespace Galaxy3D.ECS.Components;

public struct VoxelWorld<T> : IComponent where T : Voxel, new()
{
    private int _componentID;
    private bool _isActive;
    private VoxelChunk<T>[,] _worldChunks;
    VoxelRegistry<T> _voxelRegistry;
    private int _chunkXZSize;
    private int _chunkYSize;

    public VoxelWorld(int worldSize, int chunkXZSize, int chunkYSize, VoxelRegistry<T> voxelRegistry)
    {
        _worldChunks = new VoxelChunk<T>[worldSize, worldSize];
        _voxelRegistry = voxelRegistry;
        _chunkXZSize = chunkXZSize;
        _chunkYSize = chunkYSize;
    }
    
    
    public VoxelChunk<T>[,] worldChunks { get => _worldChunks; set => _worldChunks = value; }
    public VoxelRegistry<T> voxelRegistry { get => _voxelRegistry; set => _voxelRegistry = value; }
    public int componentID { get; set; }
    public bool isActive { get; set; }
    public int chunkXZSize { get; }
    public int chunkYSize { get; }
    
}