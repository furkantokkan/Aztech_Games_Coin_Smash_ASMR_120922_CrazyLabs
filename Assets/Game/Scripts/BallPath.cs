using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPath : MonoBehaviour
{
    private static BallPath instance;
    public static BallPath Instance { get { return instance; } }

    public List<BallMovement> ballsOnThisPath = new List<BallMovement>();

    [SerializeField] private SplineComputer _spline;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _yOffset;


    public static event Action<bool> _onPathUpdateFinished;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public SplineComputer GetCurrentSplineComputer()
    {
        return _spline;
    }
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

    public void AddNewPathToIndex(Vector3 pos, int IndexToAdd)
    {
        _onPathUpdateFinished?.Invoke(false);

        SplinePoint splinePoint = new SplinePoint();

        splinePoint.position = pos;
        splinePoint.normal = Vector3.up;
        splinePoint.size = 1f;
        splinePoint.color = Color.white;

        //Write the points to the spline
        _spline.SetPoint(IndexToAdd, splinePoint);
        _onPathUpdateFinished?.Invoke(true);
    }
    public void StartSpawnBalls(int ballCount)
    {
        SplineComputer _spline = BallPath.Instance.GetCurrentSplineComputer();
        float splineLength = _spline.CalculateLength();
        float distance = splineLength / ballCount;

        Debug.Log("Start Spawn Balls On Start, Ball Count is: " + ballCount);

        for (int i = 0; i < ballCount; i++)
        {
            double travel = _spline.Travel(0.0, i * distance, Spline.Direction.Forward);
            Vector3 pos = _spline.EvaluatePosition(travel);
            GameObject ball = Pool.instance.Get(PoolItems.Ball1);
            ball.gameObject.SetActive(true);
            ball.transform.position = pos;
            ball.GetComponent<BallMovement>().SetStartPosition(travel);
        }
    }
    public void SpawnBallToPath()
    {
        //float splineLength = _spline.CalculateLength();
        //double travel = _spline.Travel(0.0, splineLength / distance, Spline.Direction.Forward);
        //Vector3 nextPos = _spline.EvaluatePosition(travel);
        //Debug.DrawRay(nextPos, Vector3.up, Color.red, 10f);
    }
}
