using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolItem
{
    public GameObject prefab;
    public int amount;
    public PoolItems id;
    public bool expandAble;
}
public class Pool : MonoBehaviour
{
    public static Pool instance;
    public List<PoolItem> items;
    public List<GameObject> SpawnedItems;

    private void Awake()
    {
        instance = this;
    }
    public GameObject Get(PoolItems id)
    {
        for (int i = 0; i < SpawnedItems.Count; i++)
        {
            if (!SpawnedItems[i].activeInHierarchy && SpawnedItems[i].TryGetComponent<PoolElement>(out PoolElement poolElement))
            {
                if (poolElement.value == id)
                {
                    return SpawnedItems[i];
                }
            }
        }
        //Expand Pool if selected
        foreach (PoolItem item in items)
        {
            if (item.id == id && item.expandAble)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.AddComponent<PoolElement>().value = id;
                obj.SetActive(false);
                SpawnedItems.Add(obj);
                return obj;
            }
        }
        return null;
    }
    void Start()
    {
        SpawnedItems = new List<GameObject>();
        foreach (PoolItem item in items)
        {
            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.AddComponent<PoolElement>().value = item.id;
                obj.SetActive(false);
                SpawnedItems.Add(obj);
            }
        }
    }
}