namespace Galaxy3D.ECS;

/// <summary>
/// Generates IDs for entities and recycles released IDs.
/// IDs are generated lazily (on demand).
/// </summary>
public class IdGenerator
{
    private int _maxAmountOfIds;
    private long _nextId = 0;

    private readonly Queue<int> recycledIds = new();

    public IdGenerator(int maxAmountOfIds)
    {
        this._maxAmountOfIds = maxAmountOfIds;
    }

    public int CreateEntityID()
    {
        if (recycledIds.Count > 0)
            return recycledIds.Dequeue();

        if (_nextId > _maxAmountOfIds)
            throw new InvalidOperationException("Maximum number of entity IDs reached.");

        return (int)_nextId++;
    }

    public void ReleaseEntityID(int id)
    {
        recycledIds.Enqueue(id);
    }
    
    public int maxAmountOfIds {get => _maxAmountOfIds; set => _maxAmountOfIds = value; }
}