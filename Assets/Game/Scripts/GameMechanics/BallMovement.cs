using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private ParticleSystem _myTrail;
    [SerializeField] private SplineFollower _follower;

    private Platform platform;

    private Tween currentTween;

    public Platform Platform { set { platform = value; } }

    // Start is called before the first frame update
    void Start()
    {
        _follower.spline = platform.GetCurrentSplineComputer();
        _follower.onEndReached += OnEndOfThePath;
        ActivateSplineFollow(true);
        Platform._onPathUpdateFinished += ActivateSplineFollow;
    }
    private void OnDisable()
    {
        Platform._onPathUpdateFinished -= ActivateSplineFollow;
        _follower.onEndReached -= OnEndOfThePath;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateTrail(bool isActive)
    {
        if (isActive)
        {
            _myTrail.Play();
        }
        else
        {
            _myTrail.Stop();
        }
    }

    public void SetCurrentTween(Tween tween)
    {
        currentTween = tween;
    }
    public void KillTheCurrentTween()
    {
        if (currentTween == null)
        {
            Debug.LogWarning("Tween is null");
            return;
        }

        currentTween?.Kill();
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
