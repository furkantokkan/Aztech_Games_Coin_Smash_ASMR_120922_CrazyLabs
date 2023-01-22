using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public List<GameObject> platformLevels = new List<GameObject>();
    public List<GameObject> platformPins = new List<GameObject>();

    [SerializeField] private SplineComputer currentSpline;
    [SerializeField] private FirePlatform firePlatform;
    [SerializeField] private MoneyFloor moneyFloor;
    [SerializeField] private Transform warpHoleTransform;
    [SerializeField] private Transform warpHoleEdgeTransform;
    [SerializeField] private Transform MoneyPlatformTransform;
    [SerializeField] private float startXOffset;
    [SerializeField] private float startYOffset;
    [SerializeField] private float endOffset = 23f;

    public static Action<GameObject> OnNewBallSpawned;

    public static Action<SplineComputer> OnSplineComputerChange;

    public SplineComputer GetCurrentSplineComputer()
    {
        if (currentSpline == null)
        {
            Debug.LogError("Spline is null in platfom");
            return null;
        }
        return currentSpline;
    }
    public Transform GetWarpHole()
    {
        return warpHoleTransform;
    }
    public Transform GetMoneyPlatformEdge()
    {
        return MoneyPlatformTransform;
    }
    public Transform GetWarpHoldEdge()
    {
        return warpHoleEdgeTransform;
    }
    public Vector3 GetStartPostion()
    {
        Vector3 position = currentSpline.GetPoint(7).position;
        position.x += startXOffset;
        position.y += startYOffset;
        return position;
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
        float splineLength = currentSpline.CalculateLength() - endOffset;
        print(splineLength);
        float distance = splineLength / ballCount;

        Debug.Log("Start Spawn Balls On Start, Ball Count is: " + ballCount);

        for (int i = 0; i < ballCount; i++)
        {
            double travel = currentSpline.Travel(GameConst.START_VALUE_KEY, i * distance, Spline.Direction.Forward);
            Vector3 pos = currentSpline.EvaluatePosition(travel);
            GameObject ball = Pool.instance.Get(PoolItems.Ball2);
            ball.transform.position = pos;
            BallMovement ballMovement = ball.GetComponent<BallMovement>();
            ballMovement.SetSpline(currentSpline, true);
            ballMovement.Initialization();
            ballMovement.Platform = this;
            ballMovement.SetStartPosition(travel);
            ballMovement.ActivateSplineFollow(true);
            ball.gameObject.SetActive(true);
            GameManager.Instance.ActiveBalls.Add(ballMovement);
        }
    }
    public void SpawnNewBall(PoolItems ballType)
    {
        GameObject ball = Pool.instance.Get(ballType);
        ball.gameObject.SetActive(true);
        ball.GetComponent<BallMovement>().SetSpline(currentSpline, true);
        ball.GetComponent<BallMovement>().Initialization();
        ball.GetComponent<BallMovement>().Platform = this;
        SplineSample result = new SplineSample();
        result = currentSpline.Project(GetStartPostion(), 0, 1);
        ball.GetComponent<SplineFollower>().startPosition = result.percent;
        ball.GetComponent<BallMovement>().ActivateSplineFollow(false);
        GameManager.Instance.ActiveBalls.Add(ball.GetComponent<BallMovement>());
        OnNewBallSpawned?.Invoke(ball);
    }
    public void ActivateNewPlatformLevel(int level)
    {
        for (int i = 0; i < platformLevels.Count; i++)
        {
            if (i + 1 == level)
            {
                platformLevels[i].gameObject.SetActive(true);
                currentSpline = platformLevels[i].GetComponentInChildren<SplineComputer>();
                OnSplineComputerChange?.Invoke(currentSpline);
            }
            else
            {
                platformLevels[i].gameObject.SetActive(false);
            }
        }
    }
    public void UnlockPins(int level)
    {
        for (int i = 0; i < platformPins.Count; i++)
        {
            if (i < level)
            {
                platformPins[i].gameObject.SetActive(true);
            }
            else
            {
                platformPins[i].gameObject.SetActive(false);
            }
        }

    }
    //public void SpawnBallToPath()
    //{
    //    //float splineLength = _spline.CalculateLength();
    //    //double travel = _spline.Travel(0.0, splineLength / distance, Spline.Direction.Forward);
    //    //Vector3 nextPos = _spline.EvaluatePosition(travel);
    //    //Debug.DrawRay(nextPos, Vector3.up, Color.red, 10f);
    //}
    public void OnBallEndReach(Transform ball)
    {
        firePlatform.Shoot(ball, moneyFloor.GetRandomActiveCoin());
    }
}
