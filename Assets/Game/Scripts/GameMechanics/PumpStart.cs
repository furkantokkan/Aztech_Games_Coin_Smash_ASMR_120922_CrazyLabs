using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpStart : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer pump;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float resetAnimationTime = 0.5f;

    private static PumpStart _instance;
    public static PumpStart Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    void Start()
    {
        Platform.OnNewBallSpawned += OnAddNewBall;
    }
    private void OnDisable()
    {
        Platform.OnNewBallSpawned -= OnAddNewBall;
    }

    public void OnAddNewBall(GameObject ball)
    {
        GameManager.Instance.SetCanAddBall(false);
        GameManager.Instance.SetCanMerge(false);
        ball.transform.position = transform.position;

        StartCoroutine(WaitForOtherBalls(ball));

        float motion = 100f;
        float targetAmount = 0f;

        DOTween.To(() => motion, x => motion = x, targetAmount, animationTime).OnUpdate(delegate
        {
            pump.SetBlendShapeWeight(0, motion);
        }).OnComplete(delegate
        {
            pump.SetBlendShapeWeight(0, targetAmount);

            targetAmount = 100f;

            DOTween.To(() => motion, x => motion = x, targetAmount, resetAnimationTime).OnUpdate(delegate
            {
                pump.SetBlendShapeWeight(0, motion);
            }).OnComplete(delegate
            {
                pump.SetBlendShapeWeight(0, targetAmount);
            });

        });

    }

    private IEnumerator WaitForOtherBalls(GameObject newBall)
    {
        int checkedCount = 0;
        bool finished = false;

        while (!finished)
        {
            List<BallMovement> lookUp = new List<BallMovement>();
            BallMovement currentBallMovement = newBall.GetComponent<BallMovement>();
            lookUp.AddRange(GameManager.Instance.ActiveBalls);
            lookUp.Remove(currentBallMovement);

            print(lookUp.Count);
            for (int i = 0; i < lookUp.Count; i++)
            {
                float distance = Vector3.Distance(currentBallMovement.transform.position, new Vector3(lookUp[i].transform.position.x, lookUp[i].transform.position.y, lookUp[i].transform.position.z));
                if (distance >= GameManager.Instance.ballSpawnDistance)
                {
                    checkedCount += 1;
                }

                Debug.Log("Ball Can't Move, Current Count Is: " + checkedCount + " Current Index Is: " + i);
                yield return null;
            }

            if (checkedCount >= lookUp.Count)
            {
                Debug.Log("Ball Can Move, Current Count Is: " + checkedCount);
                currentBallMovement.GetComponent<SplineFollower>().enabled = true;
                currentBallMovement.ActivateSplineFollow(true);
                GameManager.Instance.SetCanAddBall(true);
                GameManager.Instance.SetCanMerge(true);
                finished = true;
                yield break;
            }
            else
            {
                Debug.Log("List is broked, current count is: " + checkedCount);
                checkedCount = 0;
                yield return null;
            }

            if (lookUp.Count <= 1)
            {
                currentBallMovement.ActivateSplineFollow(true);
                GameManager.Instance.SetCanAddBall(true);
                GameManager.Instance.SetCanMerge(true);
                finished = true;
                break;
            }

        }
    }
}
