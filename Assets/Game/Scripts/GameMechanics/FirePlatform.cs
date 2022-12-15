using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirePlatform : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer weapon;
    [SerializeField] Transform parrentWeapon;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float fireAnimationTime = 0.25f;
    [SerializeField] float resetAnimationTime = 0.15f;

    [SerializeField] [Range(0, 100f)] float shootOnThisMotion = 95f;
    [SerializeField] private UnityEvent onShootBall;   


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
                        onShootBall?.Invoke();
                        isFire = true;
                    }
                    weapon.SetBlendShapeWeight(0, motion);
                }).OnComplete(delegate
                {
                    weapon.SetBlendShapeWeight(0, targetAmount);
                    parrentWeapon.DOLocalRotate(new Vector3(90f, 0f, 0f), resetAnimationTime, RotateMode.Fast);
                });
            });


    }

    private void SendTheBallToTarget(Transform ball, Transform target)
    {
        BallMovement ballMovement = ball.GetComponent<BallMovement>();
        Tween shootTween = ball.DOMove(target.position, 0.3f, false).OnStart(delegate
        {
            Debug.Log("Set Fire Speed To Trail");
            ballMovement.SetTrailTime(ballMovement.GetTrailTime(true));
            ballMovement.PreventOverrideTrailTime(true);
        }).OnComplete(delegate
        {
            Debug.Log("Set Slow Speed To Trail");
            ballMovement.PreventOverrideTrailTime(false);
        }).OnKill(delegate
        {
            Debug.Log("Set Slow Speed To Trail");
            ballMovement.PreventOverrideTrailTime(false);
        });
        ballMovement.SetCurrentMoveTween(shootTween);
    }
}
