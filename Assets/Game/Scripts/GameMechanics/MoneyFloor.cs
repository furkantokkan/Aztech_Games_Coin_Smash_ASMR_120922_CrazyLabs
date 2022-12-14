using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyFloor : MonoBehaviour
{
    public List<Transform> coins = new List<Transform>();
    private List<Transform> activeCoins = new List<Transform>();

    [SerializeField] Transform platformToMove;

    private void Start()
    {
        coins.AddRange(GetComponentsInChildren<Transform>());
        coins.Remove(transform);
        activeCoins.AddRange(coins);
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

    public Transform GetRandomActiveCoin()
    {
        int index = Random.Range(0, activeCoins.Count);
        return activeCoins[index];
    }
}
