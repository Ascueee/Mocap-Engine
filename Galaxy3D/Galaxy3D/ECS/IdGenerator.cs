namespace Galaxy3D.ECS;

/// <summary>
/// Generates IDs for entities and recycles released IDs.
/// IDs are generated lazily (on demand).
/// </summary>
public class IdGenerator
{
    private readonly long maxAmountOfIds;
    private long nextId = 0;

    private readonly Queue<int> recycledIds = new();

    public IdGenerator(long maxAmountOfIds)
    {
        this.maxAmountOfIds = maxAmountOfIds;
    }

    public int CreateEntityID()
    {
        if (recycledIds.Count > 0)
            return recycledIds.Dequeue();

        if (nextId >= maxAmountOfIds)
            throw new InvalidOperationException("Maximum number of entity IDs reached.");

        return (int)nextId++;
    }

    public void ReleaseEntityID(int id)
    {
        recycledIds.Enqueue(id);
    }
}