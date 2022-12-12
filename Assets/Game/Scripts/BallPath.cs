using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPath : MonoBehaviour
{
    public List<BallMovement> ballsOnThisPath = new List<BallMovement>();

    [SerializeField] private SplineComputer _spline;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _yOffset;


    public static event Action<bool> onPathUpdateFinished;

    public Vector3 GetInitialPostion()
    {
        Vector3 position = _spline.GetPoint(0).position;
        position.x += _xOffset;
        position.y += _yOffset;
        return position;
    }
    public void RemoveMergedBall(BallMovement ball)
    {
        if (ballsOnThisPath.Contains(ball))
        {
            ballsOnThisPath.Remove(ball);
        }
    }

    public void AddNewPathToIndex(Vector3 pos)
    {
        onPathUpdateFinished?.Invoke(false);
        int newIndex = _spline.GetPoints().Length;

        SplinePoint splinePoint = new SplinePoint();

        splinePoint.position = pos;
        splinePoint.normal = Vector3.up;
        splinePoint.size = 1f;
        splinePoint.color = Color.white;

        //Write the points to the spline
        _spline.SetPoint(newIndex, splinePoint);
        onPathUpdateFinished?.Invoke(true);

    }
}
