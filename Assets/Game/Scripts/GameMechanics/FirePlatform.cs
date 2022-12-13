using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlatform : MonoBehaviour
{
    public void Shoot(Transform ball, Transform target)
    {
        ball.DOMove(target.position, 1f, false);
        Debug.Log("Shoot");
    }
}
