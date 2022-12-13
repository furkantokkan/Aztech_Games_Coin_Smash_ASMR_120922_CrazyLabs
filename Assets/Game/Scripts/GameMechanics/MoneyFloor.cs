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
    }

    public Transform GetRandomActiveCoin()
    {
        int index = Random.Range(0, activeCoins.Count);
        return activeCoins[index];
    }
}
