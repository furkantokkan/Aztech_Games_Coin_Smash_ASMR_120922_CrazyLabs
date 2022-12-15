using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpStart : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer pump;
    [SerializeField] float animationTime = 0.5f;
    [SerializeField] float resetAnimationTime = 0.5f;

    void Start()
    {
        Platform.OnNewBallSpawned += OnAddNewBall;
    }

    private void OnAddNewBall(GameObject ball)
    {
        StartCoroutine(WaitForOtherBalls(ball));
        ball.transform.position = transform.position;

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
        List<BallMovement> lookUp = new List<BallMovement>();
        BallMovement currentBallMovement = newBall.GetComponent<BallMovement>();
        lookUp.AddRange(GameManager.Instance.ActiveBalls);
        lookUp.Remove(currentBallMovement);
        int checkedCount = 0;
        bool finished = false;

        while (!finished)
        {
            for (int i = 0; i < lookUp.Count; i++)
            {
                Debug.Log("Checked: " + checkedCount);

                if (i >= lookUp.Count - 1)
                {
                    Debug.Log("End Of List, Checked Count: " + checkedCount);

                    if (checkedCount >= lookUp.Count - 1)
                    {
                        Debug.Log("Ball Can Move, Current Count Is: " + checkedCount);
                        currentBallMovement.ActivateSplineFollow(true);
                        finished = true;
                        yield break;
                    }
                    else
                    {
                        Debug.Log("List is broked, current count is: " + checkedCount);
                        checkedCount = 0;
                        break;
                    }
                }

                float distance = Vector3.Distance(newBall.transform.position, lookUp[i].transform.position);
                print("Current distance is: " + distance);
                if (distance >= GameManager.Instance.ballSpawnDistance)
                {
                    checkedCount += 1;
                }
                else
                {
                    checkedCount -= 1;
                }

                yield return null;
            }
        }
    }
}
