using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float notVisibleTime = 3f;

    public static event Action<Transform> onCoinInvisible;
    public static event Action<Transform> onCoinVisible;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            Debug.Log(other.gameObject.name + "Is hit that: " + gameObject.name);
            other.GetComponent<BallMovement>().KillTheCurrentTween();
            other.GetComponent<SphereCollider>().isTrigger = false;

            Sequence mySequence = DOTween.Sequence();
            Vector3[] vectorArray = new Vector3[] { GameManager.Instance.platform.GetWarpHoldEdge().position, GameManager.Instance.platform.GetWarpHole().position };

            mySequence.Append(other.transform.DOJump(GameManager.Instance.platform.GetMoneyPlatformEdge().position, 1f, 1, 0.5f, false).SetEase(Ease.Linear));
            mySequence.Append(other.transform.DOLocalJump(new Vector3(GameManager.Instance.platform.GetWarpHoldEdge().position.x, other.transform.position.y, other.transform.position.z), 1f, 1, 0.5f, false).SetEase(Ease.Linear));
            mySequence.Append(other.transform.DOPath(vectorArray, 2f, PathType.CatmullRom, PathMode.Full3D, 10, Color.green));
            //mySequence.Append(other.transform.DOJump(GameManager.Instance.platform.GetWarpHole().position, 1f, 1, 1, false).SetEase(Ease.Linear));

            //other.transform.DOJump(GameManager.Instance.platform.GetMoneyPlatformEdge().position, 1f, 1, 1f, false).OnComplete(delegate
            //{
            //    other.transform.DOJump(GameManager.Instance.platform.GetWarpHoldEdge().position, 1f, 1, 1f, false);
            //});
            //Vector3[] vectorArray = new Vector3[] { other.transform.position, GameManager.Instance.platform.GetWarpEdge().position, GameManager.Instance.platform.GetWarpHole().position };
            //other.transform.DOPath(vectorArray, 2f, PathType.CatmullRom, PathMode.Full3D, 10, Color.green);
            meshRenderer.enabled = false;
            onCoinInvisible?.Invoke(transform);
            Invoke("MakeVisible", notVisibleTime);
        }
    }

    private static void SetBallToInterectable(Collider other)
    {
        Rigidbody ballrigidbody = other.GetComponent<Rigidbody>();
        ballrigidbody.isKinematic = false;
        ballrigidbody.useGravity = true;
    }

    private void MakeVisible()
    {
        meshRenderer.enabled = true;
        onCoinVisible?.Invoke(transform);
    }
}
