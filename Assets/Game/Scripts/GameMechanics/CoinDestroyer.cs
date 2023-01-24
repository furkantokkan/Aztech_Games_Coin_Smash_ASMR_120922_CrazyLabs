using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDestroyer : MonoBehaviour
{
    public LayerMask targetLayer;
    public float radius = 0.5f;
    public void TargetAndDestroyCoins()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetLayer);

        foreach (Collider c in hits)
        {
            Coin coin = c.gameObject.GetComponent<Coin>();
            if (coin == null)
            {
                continue;
            }
            coin.StartDestroyTheCoin();
            PoolItems poolItems = GetComponent<PoolElement>().value;
            int index = Array.IndexOf(Enum.GetValues(poolItems.GetType()), poolItems);
            EconomyManager.Instance.EarnMoney((index + 1) * 10);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
