using DG.Tweening;
using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private TrailRenderer myTrail;
    [SerializeField] private SplineFollower follower;
    [SerializeField] private float fastTrailTime = 1.5f;
    [SerializeField] private float ShootTrailTime = 0.5f;
    [SerializeField] private float slowTrailTime = 0f;
    [SerializeField] private float trailFollowSpeed = 3.5f;

    private Platform platform;

    private Tween currentMoveTween;
    private Tween trailTween;
    public Platform Platform { set { platform = value; } }

    private bool preventOverrideTrail = false;
    private bool preventOverrideFollowMode = false;

    public static event Action onBallsStartToMove;

    // Start is called before the first frame update

    public void Initialization()
    {
        Debug.Log("Code Working");
        follower = GetComponent<SplineFollower>();
        platform = GameManager.Instance.platform;
        follower.onEndReached += OnEndOfThePath;
        follower.onMotionApplied += OnBallStartToMove;
        follower.followSpeed = trailFollowSpeed;
    }

    private void OnDisable()
    {
        follower.onEndReached -= OnEndOfThePath;
        follower.onMotionApplied -= OnBallStartToMove;
    }
    
    internal void MergeAction()
    {
        KillTheCurrentMoveTween();
        Vector3 beforeCloseFollow = transform.position;
        preventOverrideFollowMode = false;
        ActivateSplineFollow(false);
        GetComponent<SplineFollower>().enabled = false;
        transform.position = beforeCloseFollow;
    }

    // Update is called once per frame
    public void ActivateTrail(bool isActive)
    {
        if (myTrail == null)
        {
            return;
        }

        if (isActive)
        {
            trailTween?.Kill();
            myTrail.time = fastTrailTime;
        }
        else
        {
            Debug.Log("Close The Trail");
            float motion = myTrail.time;
            float targetAmount = slowTrailTime;

            trailTween = DOTween.To(() => motion, x => motion = x, targetAmount, 0.5f).OnUpdate(delegate
             {
                 SetTrailTime(motion);
             }).OnComplete(delegate
             {
                 SetTrailTime(slowTrailTime);
             });
        }
    }
    public float GetTrailTime(bool isShootTime)
    {
        if (isShootTime)
        {
            return ShootTrailTime;
        }
        else
        {
            return slowTrailTime;
        }
    }
    public void GetBackSequence()
    {
        Sequence mySequence = DOTween.Sequence();
        Vector3[] vectorArray = new Vector3[] { platform.GetWarpHoldEdge().position, platform.GetWarpHole().position };

        if (Vector3.Distance(transform.position, platform.GetWarpHoldEdge().position) > 4f)
        {
            mySequence.Append(transform.DOJump(platform.GetMoneyPlatformEdge().position, 1f, 1, 0.5f, false).SetEase(Ease.Linear));
            mySequence.Append(transform.DOLocalJump(new Vector3(platform.GetWarpHoldEdge().position.x, transform.position.y, transform.position.z), 1f, 1, 0.5f, false).SetEase(Ease.Linear));
        }
        else
        {
            mySequence.Append(transform.DOJump(platform.GetMoneyPlatformEdge().position, 1f, 1, 0.5f, false).SetEase(Ease.Linear));
            mySequence.Append(transform.DOLocalJump(new Vector3(platform.GetWarpHoldEdge().position.x, transform.position.y, transform.position.z), 1.25f, 1, 0.5f, false).SetEase(Ease.Linear));
        }

        mySequence.Append(transform.DOPath(vectorArray, 1f, PathType.CatmullRom, PathMode.TopDown2D, 10, Color.green).SetEase(Ease.Linear)).OnComplete(delegate
        {
            transform.position = platform.GetWarpHole().position;
            SetStartPosition(0);
            SetSpline(platform.GetCurrentSplineComputer(), true);
            ActivateSplineFollow(true);
            GetComponent<SphereCollider>().isTrigger = true;
            GetComponent<MergeHandler>().canMerge = true;
        });
    }
    public void PreventOverrideTrailTime(bool value)
    {
        preventOverrideTrail = value;
    }
    public void PreventOverrideFollowMode(bool value)
    {
        preventOverrideFollowMode = value;
    }
    public void SetTrailTime(float time)
    {
        if (preventOverrideTrail)
        {
            return;
        }
        myTrail.time = time;
    }
    public void SetCurrentMoveTween(Tween tween)
    {
        currentMoveTween = tween;
    }
    public void KillTheCurrentMoveTween()
    {
        if (currentMoveTween == null)
        {
            Debug.LogWarning("Tween is null");
            return;
        }

        currentMoveTween?.Kill();
    }

    #region Spline
    public void ActivateSplineFollow(bool followMode = true)
    {
        if (preventOverrideFollowMode)
        {
            return;
        }
        follower.follow = followMode;
    }

    public void SetStartPosition(double pos)
    {
        follower.startPosition = pos;
    }

    public void OnBallStartToMove()
    {
        onBallsStartToMove?.Invoke();
    }
    public void OnEndOfThePath(double value)
    {
        ActivateSplineFollow(false);
        platform.OnBallEndReach(transform);
    }
    //public void SetSplineFollowerSpeed(float value)
    //{
    //    float current = _follower.followSpeed;
    //    speed?.Kill();
    //    speed = DOTween.To(() => current, x => current = x, value, 0.2f)
    //   .OnUpdate(() =>
    //   {
    //       _follower.followSpeed = current;
    //   });
    //}
    public void SetSpline(SplineComputer splineComputer, bool rebuild)
    {
        follower.spline = splineComputer;
        if (rebuild)
        {
            SplineSample result = new SplineSample();
            result = splineComputer.Project(transform.position, 0, 1);
            follower.RebuildImmediate();
            follower.Restart(result.percent);
        }
    }
    #endregion
}
