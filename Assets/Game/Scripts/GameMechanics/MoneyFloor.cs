using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyFloor : MonoBehaviour
{
    private static MoneyFloor _instance;
    public static MoneyFloor Instance { get { return _instance; } }

    public List<Transform> coins = new List<Transform>();
    private List<Transform> activeCoins = new List<Transform>();
    [SerializeField] private List<Transform> childs = new List<Transform>();
    [SerializeField] TargetData[] targetsToShoot;

    [SerializeField] Transform platformToMove;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Start()
    {
        Coin.onCoinInvisible += RemoveFromList;
        Coin.onCoinVisible += AddToList;
    }
    private void OnDisable()
    {
        Coin.onCoinInvisible -= RemoveFromList;
        Coin.onCoinVisible -= AddToList;
    }

    public void RemoveFromList(Transform transform)
    {
        activeCoins.Remove(transform);
    }

    public void AddToList(Transform transform)
    {
        activeCoins.Add(transform);
    }

    public void ActivateTargetToShoot(int buildingIndex, int incomeIndex)
    {
        coins.Clear();
        activeCoins.Clear();

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].gameObject.SetActive(false);
            if (i == childs.Count - 1)
            {
                targetsToShoot[buildingIndex].targets[incomeIndex].gameObject.SetActive(true);
                coins.AddRange(targetsToShoot[buildingIndex].targets[incomeIndex].GetComponentsInChildren<Transform>());
                coins.Remove(targetsToShoot[buildingIndex].targets[incomeIndex].transform);
                activeCoins.AddRange(coins);
            }
        }
    }


    public Transform GetRandomActiveCoin()
    {
        int index = Random.Range(0, activeCoins.Count);
        return activeCoins[index];
    }
}
[System.Serializable]
public class TargetData
{
    public GameObject[] targets;
}