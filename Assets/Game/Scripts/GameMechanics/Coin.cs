using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float notVisibleTime = 3f;

    public static event Action<Transform> onCoinInvisible;
    public static event Action<Transform> onCoinVisible;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            Debug.Log(other.gameObject.name + "Is hit that: " + gameObject.name);
            other.GetComponent<BallMovement>().KillTheCurrentTween();
            meshRenderer.enabled = false;
            onCoinInvisible?.Invoke(transform);
            Invoke("MakeVisible", notVisibleTime);
        }
    }

    private void MakeVisible()
    {
        meshRenderer.enabled = true;
        onCoinVisible?.Invoke(transform);
    }
}
