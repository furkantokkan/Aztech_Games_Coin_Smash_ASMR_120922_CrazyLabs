using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAnimationSystem : MonoBehaviour
{
    private static BallAnimationSystem _instance;
    public static BallAnimationSystem Instance { get { return _instance; } }

    public Transform Path1;
    public Transform Path2;
    public Transform Path3;

    [SerializeField] private Animator animator;
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private float getBackTime = 5f;
    [SerializeField] private float pathFinishTime = 3f;
    [SerializeField] private float angeYOffset = 4f;
    [SerializeField] private float angeXOffset = 4f;

    public Transform BallSpawnPosition;

    private Vector3[] Path1Positions = new Vector3[3];
    private Vector3[] Path2Positions = new Vector3[3];
    private Vector3[] Path3Positions = new Vector3[3];
    private Vector3[] GetBackPath = new Vector3[3];

    private BallMovement[] ballsArray;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    public void ActivateMerge(BallMovement[] balls)
    {
        ballsArray = balls;

        for (int i = 0; i < balls.Length; i++)
        {
            ballsArray[i].MergeAction();
        }

        Path1Positions[0] = ballsArray[0].transform.position;
        Path2Positions[0] = ballsArray[1].transform.position;
        Path3Positions[0] = ballsArray[2].transform.position;
        GetBackPath[0] = BallSpawnPosition.position;

        Path1Positions[1] = Lerp(ballsArray[0].transform.position, Path1.position, 0.53f) + new Vector3(0f, angeYOffset, 0f);
        Path2Positions[1] = Lerp(ballsArray[1].transform.position, Path2.position, 0.53f) + new Vector3(angeXOffset, angeYOffset, 0f);
        Path3Positions[1] = Lerp(ballsArray[2].transform.position, Path3.position, 0.53f) + new Vector3(-angeXOffset, angeYOffset, 0f);
        GetBackPath[1] = Lerp(BallSpawnPosition.position, PumpStart.Instance.transform.position, 0.53f) + new Vector3(0f, angeYOffset, 0f); ;

        Path1Positions[2] = Path1.position;
        Path2Positions[2] = Path2.position;
        Path3Positions[2] = Path3.position;
        GetBackPath[2] = PumpStart.Instance.transform.position;

        Sequence ballPathAnimations = DOTween.Sequence();

        ballPathAnimations.Join(ballsArray[0].transform.DOPath(Path1Positions, pathFinishTime, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None));
        ballPathAnimations.Join(ballsArray[1].transform.DOPath(Path2Positions, pathFinishTime, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None));
        ballPathAnimations.Join(ballsArray[2].transform.DOPath(Path3Positions, pathFinishTime, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None));
        ballPathAnimations.OnComplete(() =>
        {
            ballsArray[0].transform.position = Path1.position;
            ballsArray[1].transform.position = Path2.position;
            ballsArray[2].transform.position = Path3.position;

            ballsArray[0].transform.SetParent(Path1);
            ballsArray[1].transform.SetParent(Path2);
            ballsArray[2].transform.SetParent(Path3);
            animator.SetBool("Rotate", true);
            animator.speed = animationSpeed;
        });
    }
    /*Call Back*/
    private void GetBackSequence()
    {

        //ballRotateAnimations.Join(Path1.transform.DOMove(BallSpawnPosition.position, getBackTime, false));
        //ballRotateAnimations.Join(Path2.transform.DOMove(BallSpawnPosition.position, getBackTime, false));
        //ballRotateAnimations.Join(Path3.transform.DOMove(BallSpawnPosition.position, getBackTime, false));

        //ballRotateAnimations.OnComplete(() =>
        //{
        Debug.Log("Get Back To Platform");
        animator.SetBool("Rotate", false);

        GameObject currentBall = Pool.instance.Get(ballsArray[0].GetComponent<MergeHandler>().evolveToThis);
        currentBall.gameObject.SetActive(true);
        currentBall.transform.position = BallSpawnPosition.position;
        currentBall.GetComponent<SplineFollower>().enabled = false;

        currentBall.transform.DOPath(GetBackPath, pathFinishTime, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None).OnComplete(delegate
        {
            OnFinishPath(currentBall);
        });

        for (int i = 0; i < ballsArray.Length; i++)
        {
            ballsArray[i].transform.SetParent(Pool.instance.transform);
            ballsArray[i].gameObject.SetActive(false);
        }
    }
    private void OnFinishPath(GameObject currentBall)
    {
        currentBall.GetComponent<SplineFollower>().enabled = true;
        currentBall.gameObject.SetActive(false);

        GameObject ball = Pool.instance.Get(currentBall.GetComponent<PoolElement>().value);
        ball.gameObject.SetActive(true);
        ball.GetComponent<SplineFollower>().spline = GameManager.Instance.platform.GetCurrentSplineComputer();
        ball.GetComponent<BallMovement>().Initialization();
        ball.GetComponent<BallMovement>().Platform = GameManager.Instance.platform;
        SplineSample result = new SplineSample();
        result = GameManager.Instance.platform.GetCurrentSplineComputer().Project(GameManager.Instance.platform.GetStartPostion(), 0, 1);
        ball.GetComponent<SplineFollower>().startPosition = result.percent;
        ball.GetComponent<BallMovement>().ActivateSplineFollow(false);
        GameManager.Instance.ActiveBalls.Add(ball.GetComponent<BallMovement>());
        Platform.OnNewBallSpawned?.Invoke(ball);
    }
    Vector3 Lerp(Vector3 start, Vector3 end, float percent)
    {
        return (start + percent * (end - start));
    }
}
