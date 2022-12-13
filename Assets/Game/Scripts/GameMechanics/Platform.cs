using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public List<BallMovement> ballsOnThisPath = new List<BallMovement>();

    [SerializeField] private SplineComputer spline;
    [SerializeField] private FirePlatform firePlatform;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;


    public static event Action<bool> _onPathUpdateFinished;

    private void Awake()
    {

    }
    public SplineComputer GetCurrentSplineComputer()
    {
        return spline;
    }
    public Vector3 GetInitialPostion()
    {
        Vector3 position = spline.GetPoint(0).position;
        position.x += xOffset;
        position.y += yOffset;
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
        spline.SetPoint(IndexToAdd, splinePoint);
        _onPathUpdateFinished?.Invoke(true);
    }
    public void StartSpawnBalls(int ballCount)
    {
        float splineLength = spline.CalculateLength();
        float distance = splineLength / ballCount;

        Debug.Log("Start Spawn Balls On Start, Ball Count is: " + ballCount);

        for (int i = 0; i < ballCount; i++)
        {
            double travel = spline.Travel(0.0, i * distance, Spline.Direction.Forward);
            Vector3 pos = spline.EvaluatePosition(travel);
            GameObject ball = Pool.instance.Get(PoolItems.Ball1);
            ball.transform.position = pos;
            BallMovement ballMovement = ball.GetComponent<BallMovement>();
            ballMovement.Platform = this;
            ballMovement.SetStartPosition(travel);
            ballsOnThisPath.Add(ballMovement);
            ball.gameObject.SetActive(true);
        }
    }
    public void SpawnBallToPath()
    {
        //float splineLength = _spline.CalculateLength();
        //double travel = _spline.Travel(0.0, splineLength / distance, Spline.Direction.Forward);
        //Vector3 nextPos = _spline.EvaluatePosition(travel);
        //Debug.DrawRay(nextPos, Vector3.up, Color.red, 10f);
    }
    public void OnBallEndReach(Transform ball)
    {
        firePlatform.Shoot(ball);
    }
}
