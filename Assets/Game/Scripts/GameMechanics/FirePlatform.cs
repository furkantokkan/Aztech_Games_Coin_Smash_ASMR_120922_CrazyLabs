using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlatform : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer weapon;
    [SerializeField] Transform parrentWeapon;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float fireAnimationTime = 0.25f;

    [SerializeField] [Range(0, 100f)] float shootOnThisMotion = 95f;

    public void Shoot(Transform ball, Transform target)
    {
        float motion = 0f;
        float targetAmount = 100f;

        parrentWeapon.transform.DOLookAt(target.position, animationTime, AxisConstraint.Z, Vector3.up);

        DOTween.To(() => motion, x => motion = x, targetAmount, animationTime).OnUpdate(delegate
            {
                weapon.SetBlendShapeWeight(0, motion);
            }).OnComplete(delegate
            {
                weapon.SetBlendShapeWeight(0, targetAmount);
                targetAmount = 0;
                bool isFire = false;

                DOTween.To(() => motion, x => motion = x, targetAmount, fireAnimationTime).OnUpdate(delegate
                {
                    if (motion <= shootOnThisMotion && !isFire)
                    {
                        SendTheBallToTarget(ball, target);
                        isFire = true;
                    }
                    weapon.SetBlendShapeWeight(0, motion);
                }).OnComplete(delegate
                {
                    weapon.SetBlendShapeWeight(0, targetAmount);
                });
            });


    }

    private void SendTheBallToTarget(Transform ball, Transform target)
    {
        Tween shootTween = ball.DOMove(target.position, 0.3f, false);
        ball.GetComponent<BallMovement>().SetCurrentTween(shootTween);
    }
}
