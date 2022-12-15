using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetMoneyAmountToText(int amount)
    {
        text.text = amount.ToString() + "$";
    }
}
