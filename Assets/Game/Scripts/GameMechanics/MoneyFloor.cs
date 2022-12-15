using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyFloor : MonoBehaviour
{
    public List<Transform> coins = new List<Transform>();
    private List<Transform> activeCoins = new List<Transform>();
    [SerializeField] TargetData[] targetsToShoot;

    [SerializeField] Transform platformToMove;

    private void Start()
    {
        coins.AddRange(GetComponentsInChildren<Transform>());
        coins.Remove(transform);
        activeCoins.AddRange(coins);
        Coin.onCoinInvisible += RemoveFromList;
        Coin.onCoinVisible += AddToList;
        ActivateTargetToShoot(0, 0);
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

        List<Transform> allChilds = new List<Transform>();
        allChilds.AddRange(GetComponentsInChildren<Transform>());
        allChilds.Remove(transform);
        for (int i = 0; i < allChilds.Count; i++)
        {
            allChilds[i].gameObject.SetActive(false);
            if (i == allChilds.Count - 1)
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