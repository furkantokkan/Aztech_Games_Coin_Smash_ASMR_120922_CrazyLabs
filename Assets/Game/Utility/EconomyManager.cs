using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    private static EconomyManager instance;
    public static EconomyManager Instance { get { return instance; } }

    private int currentMoney = 0;

    public event Action onAddMoney;
    public event Action onSpendMoney;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Initialize();
    }

    private void Initialize()
    {
        currentMoney = PlayerPrefs.GetInt(GameConst.MONEY_KEY, 0);
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        PlayerPrefs.SetInt(GameConst.MONEY_KEY, currentMoney);
        onAddMoney?.Invoke();
    }

    public void SpendMoney(int amount)
    {
        currentMoney -= amount;
        PlayerPrefs.SetInt(GameConst.MONEY_KEY, currentMoney);
        onSpendMoney?.Invoke();
    }
}
