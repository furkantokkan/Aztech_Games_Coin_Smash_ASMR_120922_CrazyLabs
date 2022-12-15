using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] private Transform arm;
    [SerializeField] private bool isPositive = true;
    [SerializeField] private float turnValue = 75f;
    [SerializeField] private float turnTime = 0.3f;
    [SerializeField] private float turnBackTime = 0.1f;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] private Ease getBackEaseType = Ease.Linear;
    [SerializeField] private GameObject Effect;
    private Tween ballTween;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            var _effect = Instantiate(Effect, transform.position + Vector3.up, Quaternion.identity);
            PoolItems poolItems = other.GetComponent<PoolElement>().value;
            int index = Array.IndexOf(Enum.GetValues(poolItems.GetType()), poolItems);
            _effect.GetComponent<MoneyEffect>().SetMoneyAmountToText(index+1);
            GetComponent<AudioSource>().Play();
            ballTween?.Kill();
            ballTween = arm.DOLocalRotate(new Vector3(0f, 0f, turnValue), turnTime, RotateMode.Fast).SetEase(easeType).OnComplete(delegate
            {
                arm.DOLocalRotate(Vector3.zero, turnBackTime, RotateMode.Fast).SetEase(getBackEaseType);
            });
            //gain money
        }
    }

    private void Update()
    {
        if (UIManager.Instance.material)
            GetComponent<Renderer>().material = UIManager.Instance.FirstMaterials[1];
        else GetComponent<Renderer>().material = UIManager.Instance.SecondMaterials[1];
    }
}
