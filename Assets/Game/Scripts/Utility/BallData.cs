using System.Collections.Generic;

[System.Serializable]
public class BallData
{
    public List<PoolItems> BallIds = new List<PoolItems>();

    public void AddItem(PoolItems id)
    {
        BallIds.Add(id);
    }

    public void RemoveItem(PoolItems id)
    {
        if (BallIds.Contains(id))
        {
            BallIds.Remove(id);
        }
    }
}