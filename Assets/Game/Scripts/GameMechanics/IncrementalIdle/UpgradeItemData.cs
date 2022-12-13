using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Shop/Upgradeable Item")]
public class UpgradeItemData : ScriptableObject
{

    [Header("Item Data")]
    public string name;
    public float value;
    public AnimationCurve valueCurve;
    public float minValue;
    public float maxValue;
    public int currentLevel;
    public int maxLevel;

    [Header("Price")]
    public float startPrice;
    public float costPerUpdate;
    public float coefficient;
    public float fixedNumber;
    private float currentPrice;

    public float Progression => Mathf.InverseLerp(0, maxLevel, currentLevel);

    public int CurrentPrice => (int)currentPrice;

    public UnityAction OnLevelUp;

    private void SetLevel()
    {
        PlayerPrefs.SetInt(name + "lvl", currentLevel);
    }

    public void Initialize()
    {
        currentLevel = PlayerPrefs.GetInt(name + "lvl", 0);
        currentPrice = PlayerPrefs.GetFloat(name + "Price", startPrice);
        value = Mathf.Lerp(minValue, maxValue, valueCurve.Evaluate(currentLevel / (float)maxLevel));
    }

    public bool Upgrade()
    {
        if (currentLevel == maxLevel || EconomyManager.Instance.GetCurrentMoney() < currentPrice)
            return false;

        // Spend Money
        EconomyManager.Instance.SpendMoney(CurrentPrice);

        // Update Level
        currentLevel++;
        PlayerPrefs.SetInt(name + "lvl", currentLevel);

        // Update value
        value = Mathf.Lerp(minValue, maxValue, valueCurve.Evaluate(currentLevel / (float)maxLevel));

        // Update Price
        currentPrice += Mathf.RoundToInt(costPerUpdate * coefficient * currentLevel / fixedNumber);
        PlayerPrefs.SetFloat(name + "Price", currentPrice);

        MMVibrationManager.Haptic(HapticTypes.Success);

        OnLevelUp?.Invoke();

        return true;
    }

    public void ResetData()
    {
        PlayerPrefs.SetInt(name + "lvl", 0);
        PlayerPrefs.SetFloat(name + "Price", startPrice);
    }
}

