using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float notVisibleTime = 3f;
    [SerializeField] private List<GameObject> childs = new List<GameObject>();

    public static event Action<Transform> onCoinInvisible;
    public static event Action<Transform> onCoinVisible;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            BallMovement ballMovement = other.GetComponent<BallMovement>();
            ballMovement.KillTheCurrentMoveTween();
            ballMovement.GetBackSequence();
            other.GetComponent<SphereCollider>().isTrigger = false;
            PoolItems poolItems = other.GetComponent<PoolElement>().value;
            int index = Array.IndexOf(Enum.GetValues(poolItems.GetType()), poolItems);
            EconomyManager.Instance.EarnMoney((index+1) * 6);
            //mySequence.Append(other.transform.DOJump(GameManager.Instance.platform.GetWarpHole().position, 1f, 1, 1, false).SetEase(Ease.Linear));

            //other.transform.DOJump(GameManager.Instance.platform.GetMoneyPlatformEdge().position, 1f, 1, 1f, false).OnComplete(delegate
            //{
            //    other.transform.DOJump(GameManager.Instance.platform.GetWarpHoldEdge().position, 1f, 1, 1f, false);
            //});
            //Vector3[] vectorArray = new Vector3[] { other.transform.position, GameManager.Instance.platform.GetWarpEdge().position, GameManager.Instance.platform.GetWarpHole().position };
            //other.transform.DOPath(vectorArray, 2f, PathType.CatmullRom, PathMode.Full3D, 10, Color.green);
            meshRenderer.enabled = false;
            onCoinInvisible?.Invoke(transform);
            GetComponent<MeshCollider>().enabled = false;

            StartCoroutine(DestructionProcess());
        }
    }
    
    private IEnumerator DestructionProcess()
    {
        foreach (GameObject item in childs)
        {
            item.SetActive(true);
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.onUnitSphere * 100f, ForceMode.Impulse);
            item.transform.DOScale(0.1f, notVisibleTime).SetEase(Ease.InCirc).SetDelay(0.12f);
        }

        yield return new WaitForSeconds(notVisibleTime);

        foreach (GameObject item in childs)
        {
            item.SetActive(false);
            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.transform.localScale = Vector3.one;
            item.transform.position = Vector3.zero;
        }
        MakeVisible();
    }

    //private static void SetBallToInterectable(Collider other)
    //{
    //    Rigidbody ballrigidbody = other.GetComponent<Rigidbody>();
    //    ballrigidbody.isKinematic = false;
    //    ballrigidbody.useGravity = true;
    //}

    private void MakeVisible()
    {
        GetComponent<MeshCollider>().enabled = true;
        meshRenderer.enabled = true;
        onCoinVisible?.Invoke(transform);
    }
}
