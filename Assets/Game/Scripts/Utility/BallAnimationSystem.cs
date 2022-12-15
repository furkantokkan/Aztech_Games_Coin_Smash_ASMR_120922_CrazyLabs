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

    public Animator Animator;
    public Transform AnimationParent;

    public float AnimationSpeed;
    public Transform BallSpawnPosition;

    private Vector3[] Path1Positions = new Vector3[2];
    private Vector3[] Path2Positions = new Vector3[2];
    private Vector3[] Path3Positions = new Vector3[2];

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    public void ActivateMerge(BallMovement[] balls)
    {

        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].MergeAction();
        }

        Path1Positions[0] = balls[0].transform.position;
        Path2Positions[0] = balls[1].transform.position;
        Path3Positions[0] = balls[2].transform.position;

        Path1Positions[1] = Path1.position;
        Path2Positions[1] = Path2.position;
        Path3Positions[1] = Path3.position;

        Sequence ballAnimations = DOTween.Sequence();

        ballAnimations.Join(balls[0].transform.DOPath(Path1Positions, AnimationSpeed, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None));
        ballAnimations.Join(balls[1].transform.DOPath(Path2Positions, AnimationSpeed, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None));
        ballAnimations.Join(balls[2].transform.DOPath(Path3Positions, AnimationSpeed, PathType.CatmullRom).SetEase(Ease.Linear).SetLookAt(0.05f).SetOptions(false, AxisConstraint.None, AxisConstraint.None));
        ballAnimations.OnComplete(() =>
        {
            BallSpawnPosition.transform.DORotate(new Vector3(360f, 360f, 360f), 1.5f, RotateMode.FastBeyond360).SetLoops(1).OnStart(delegate
            {
                balls[0].transform.DOMove(BallSpawnPosition.position, 1f, false);
                balls[1].transform.DOMove(BallSpawnPosition.position, 1f, false);
                balls[2].transform.DOMove(BallSpawnPosition.position, 1f, false);
            }).OnComplete(delegate
             {
                 BallMovement currentBall = Pool.instance.Get(balls[0].GetComponent<MergeHandler>().evolveToThis).GetComponent<BallMovement>();
                 currentBall.GetComponent<SplineFollower>().enabled = false;
                 currentBall.gameObject.SetActive(true);
                 currentBall.transform.position = BallSpawnPosition.position;
                 currentBall.transform.DOMove(PumpStart.Instance.transform.position, 2f, false).OnComplete(delegate
                 {
                     currentBall.gameObject.SetActive(false);
                     GameManager.Instance.ActiveBalls.Remove(currentBall);
                     GameManager.Instance.platform.SpawnNewBall(balls[0].GetComponent<MergeHandler>().evolveToThis);
                 });
                 for (int i = 0; i < balls.Length; i++)
                 {
                     balls[i].gameObject.SetActive(false);
                 }
             });

        });
    }
}
