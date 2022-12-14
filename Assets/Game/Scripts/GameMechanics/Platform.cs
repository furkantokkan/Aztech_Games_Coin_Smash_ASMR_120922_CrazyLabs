using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public List<BallMovement> ballsOnThisPath = new List<BallMovement>();

    public List<GameObject> platformLevels = new List<GameObject>();

    [SerializeField] private SplineComputer currentSpline;
    [SerializeField] private FirePlatform firePlatform;
    [SerializeField] private MoneyFloor moneyFloor;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    private void Awake()
    {

    }
    public SplineComputer GetCurrentSplineComputer()
    {
        return currentSpline;
    }
    public Vector3 GetInitialPostion()
    {
        Vector3 position = currentSpline.GetPoint(0).position;
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

    //public void AddNewPathToIndex(Vector3 pos, int IndexToAdd)
    //{
    //    onPathUpdateFinished?.Invoke(false);

    //    SplinePoint splinePoint = new SplinePoint();

    //    splinePoint.position = pos;
    //    splinePoint.normal = Vector3.up;
    //    splinePoint.size = 1f;
    //    splinePoint.color = Color.white;

    //    //Write the points to the spline
    //    currentSpline.SetPoint(IndexToAdd, splinePoint);
    //    onPathUpdateFinished?.Invoke(true);
    //}
    public void StartSpawnBalls(int ballCount)
    {
        float splineLength = currentSpline.CalculateLength();
        float distance = splineLength / ballCount;

        Debug.Log("Start Spawn Balls On Start, Ball Count is: " + ballCount);

        for (int i = 0; i < ballCount; i++)
        {
            double travel = currentSpline.Travel(0.0, i * distance, Spline.Direction.Forward);
            Vector3 pos = currentSpline.EvaluatePosition(travel);
            GameObject ball = Pool.instance.Get(PoolItems.Ball1);
            ball.transform.position = pos;
            BallMovement ballMovement = ball.GetComponent<BallMovement>();
            ballMovement.Platform = this;
            ballMovement.SetStartPosition(travel);
            ballsOnThisPath.Add(ballMovement);
            ball.gameObject.SetActive(true);
        }
    }
    public void ActivateNewPlatformLevel(int level)
    {
        for (int i = 0; i < platformLevels.Count; i++)
        {
            if (i + 1 == level)
            {
                platformLevels[i].gameObject.SetActive(true);
                currentSpline = platformLevels[i].GetComponentInChildren<SplineComputer>();
            }
            else
            {
                platformLevels[i].gameObject.SetActive(false);
            }
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
        firePlatform.Shoot(ball, moneyFloor.GetRandomActiveCoin());
    }
}
