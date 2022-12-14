using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private TrailRenderer myTrail;
    [SerializeField] private SplineFollower _follower;
    [SerializeField] private float fastTrailTime = 1.5f;
    [SerializeField] private float ShootTrailTime = 0.5f;
    [SerializeField] private float slowTrailTime = 0f;

    private Platform platform;

    private Tween currentMoveTween;
    private Tween trailTween;
    public Platform Platform { set { platform = value; } }

    private bool preventOverrideTrail = false;

    // Start is called before the first frame update
    void Start()
    {
        _follower.spline = platform.GetCurrentSplineComputer();
        _follower.onEndReached += OnEndOfThePath;
        _follower.followSpeed = 3f;
        ActivateSplineFollow(true);
    }
    private void OnDisable()
    {
        _follower.onEndReached -= OnEndOfThePath;
    }

    // Update is called once per frame
    void Update()
    {
        print(myTrail.time);
    }
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

        mySequence.Append(transform.DOJump(platform.GetMoneyPlatformEdge().position, 1f, 1, 0.5f, false).SetEase(Ease.Linear));
        mySequence.Append(transform.DOLocalJump(new Vector3(platform.GetWarpHoldEdge().position.x, transform.position.y, transform.position.z), 1f, 1, 0.5f, false).SetEase(Ease.Linear));
        mySequence.Append(transform.DOPath(vectorArray, 1f, PathType.CatmullRom, PathMode.Full3D, 10, Color.green).SetEase(Ease.Linear)).OnComplete(delegate
        {
            transform.position = platform.GetWarpHole().position;
            SetStartPosition(0);
            SetSpline(platform.GetCurrentSplineComputer());
            ActivateSplineFollow(true);
            GetComponent<SphereCollider>().isTrigger = true;
        });
    }
    public void PreventOverrideTrailTime(bool value)
    {
        preventOverrideTrail = value;
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
        _follower.follow = followMode;
    }

    public void SetStartPosition(double pos)
    {
        _follower.startPosition = pos;
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
    public void SetSpline(SplineComputer splineComputer)
    {
        _follower.spline = splineComputer;
        SplineSample result = new SplineSample();
        result = splineComputer.Project(_follower.transform.position, 0, 1);
        _follower.RebuildImmediate();
        _follower.Restart(result.percent);
    }
    #endregion
}
