namespace Galaxy3D.ECS;

/// <summary>
/// Generates IDS for objects as well as keeps what ids have been used and recycled to be used later
/// </summary>
public class IdGenerator
{
    private int maxAmountOfIds;
    Queue<int> idQue = new Queue<int>();
    public IdGenerator(int maxAmountOfIds)
    {
        this.maxAmountOfIds = maxAmountOfIds;
        PopulateIDQueue();
    }

    public int Dequeue()
    {
        return idQue.Dequeue();
    }

    public void Enqueue(int id)
    {
        idQue.Enqueue(id);
    }
    
    void PopulateIDQueue()
    {
        idQue.Clear();
        for (int i = 0; i < maxAmountOfIds; i++)
        {
            idQue.Enqueue(i);
        }
    }
}