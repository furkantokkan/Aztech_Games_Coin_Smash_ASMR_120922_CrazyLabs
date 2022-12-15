using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        EconomyManager.Instance.onAddMoney += UpdateText;
        EconomyManager.Instance.onSpendMoney += UpdateText;
        UpdateText();
    }

    private void UpdateText()
    {
        text.text = EconomyManager.Instance.GetCurrentMoney().ToString();
    }

}
