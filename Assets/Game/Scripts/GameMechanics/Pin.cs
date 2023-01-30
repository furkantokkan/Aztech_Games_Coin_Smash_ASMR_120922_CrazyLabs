using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] private Transform arm;
    [SerializeField] private float turnValue = 75f;
    [SerializeField] private float turnTime = 0.3f;
    [SerializeField] private float turnBackTime = 0.1f;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] private Ease getBackEaseType = Ease.Linear;
    private Tween ballTween;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            var _effect = Pool.instance.Get(PoolItems.MoneyVFX);
            _effect.gameObject.SetActive(true);
            _effect.transform.position = arm.position + Vector3.up;
            _effect.transform.rotation = Quaternion.identity;
            PoolItems poolItems = other.GetComponent<PoolElement>().value;
            int index = Array.IndexOf(Enum.GetValues(poolItems.GetType()), poolItems);
            _effect.GetComponent<MoneyEffect>().SetMoneyAmountToText(index + 1);
            GetComponent<AudioSource>().Play();
            ballTween?.Kill();
            ballTween = arm.DOLocalRotate(new Vector3(0f, 0f, turnValue), turnTime, RotateMode.Fast).SetEase(easeType).OnComplete(delegate
            {
                arm.DOLocalRotate(Vector3.zero, turnBackTime, RotateMode.Fast).SetEase(getBackEaseType);
            });
            int value = (index + 1) * 3;
            EconomyManager.Instance.EarnMoney(value);
            _effect.GetComponent<MoneyEffect>().SetMoneyAmountToText(value);
        }
    }
}
