using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] float notVisibleTime = 3f;
    [SerializeField] float force = 85f;
    [SerializeField] private List<GameObject> childs = new List<GameObject>();

    public static event Action<Transform> onCoinInvisible;
    public static event Action<Transform> onCoinVisible;

    private Tween destroyProcess;

    private List<Vector3> initalPos = new List<Vector3>();
    private List<Vector3> initalLocalPos = new List<Vector3>();
    private List<Vector3> initalLocalScale = new List<Vector3>();
    private List<Quaternion> initalRot = new List<Quaternion>();

    private void Start()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            initalPos.Add(childs[i].transform.position);
            initalLocalPos.Add(childs[i].transform.localPosition);
            initalLocalScale.Add(childs[i].transform.localScale);
            initalRot.Add(childs[i].transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            BallMovement ballMovement = other.GetComponent<BallMovement>();
            ballMovement.KillTheCurrentMoveTween();
            ballMovement.GetBackSequence();
            other.GetComponent<CoinDestroyer>().TargetAndDestroyCoins();
            other.GetComponent<SphereCollider>().isTrigger = false;
        }
    }

    public void StartDestroyTheCoin()
    {
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

    private IEnumerator DestructionProcess()
    {
        foreach (GameObject item in childs)
        {
            item.SetActive(true);
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.onUnitSphere * force, ForceMode.Impulse);
            destroyProcess = item.transform.DOScale(0.1f, notVisibleTime).SetEase(Ease.InCirc).SetDelay(0.12f);
            yield return null;
        }

        yield return new WaitForSeconds(notVisibleTime);

        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].SetActive(false);
            childs[i].GetComponent<Rigidbody>().useGravity = false;
            childs[i].GetComponent<Rigidbody>().isKinematic = true;
            childs[i].transform.localScale = initalLocalScale[i];
            childs[i].transform.localPosition = initalLocalPos[i];
            childs[i].transform.position = initalPos[i];
            childs[i].transform.rotation = initalRot[i];
            yield return null;
        }

        yield return new WaitForEndOfFrame();

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
        destroyProcess?.Kill();
        GetComponent<MeshCollider>().enabled = true;
        meshRenderer.enabled = true;
        onCoinVisible?.Invoke(transform);
    }
}
