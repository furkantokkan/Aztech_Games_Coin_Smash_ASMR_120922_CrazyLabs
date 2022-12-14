using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlatform : MonoBehaviour
{
    public void Shoot(Transform ball, Transform target)
    {
        Tween shootTween;
        shootTween = ball.DOMove(target.position, 0.3f, false);
        ball.GetComponent<BallMovement>().SetCurrentTween(shootTween);
    }
}
