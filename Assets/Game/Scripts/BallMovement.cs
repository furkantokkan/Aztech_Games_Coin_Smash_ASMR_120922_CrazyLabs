using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private ParticleSystem _myTrail;
    [SerializeField] private SplineFollower _follower;

    private Tween speed;

    // Start is called before the first frame update
    void Start()
    {
        BallPath.onPathUpdateFinished += ActivateSplineFollow;
    }
    private void OnDisable()
    {
        BallPath.onPathUpdateFinished -= ActivateSplineFollow;
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

    #region Spline
    public void ActivateSplineFollow(bool followMode = true)
    {
        _follower.follow = followMode;
    }

    public void SetSplineFollowerSpeed(float value)
    {
        float current = _follower.followSpeed;
        speed?.Kill();
        speed = DOTween.To(() => current, x => current = x, value, 0.2f)
       .OnUpdate(() =>
       {
           _follower.followSpeed = current;
       });
    }
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
