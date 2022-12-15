using System;
using UnityEngine;


public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private UpgradeItemData[] itemDatas;
    [SerializeField] private UpgradeButton[] upgradeButtons;

    public UpgradeItemData[] ItemDatas { get => itemDatas; set => itemDatas = value; }

    private void Start()
    {
        InitializeUpgrade();
        EconomyManager.Instance.onAddMoney += InitializeUpgrade;
        EconomyManager.Instance.onSpendMoney += InitializeUpgrade;
    }
    private void OnDisable()
    {
        EconomyManager.Instance.onAddMoney += InitializeUpgrade;
        EconomyManager.Instance.onSpendMoney += InitializeUpgrade;
    }
    public void InitializeUpgrade()
    {
        InitializeItems();
        InitializeButtons();
    }

    private void OnLevelSuccess()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            itemDatas[i].ResetData();
        }
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            upgradeButtons[i].Initialize();
        }
    }

    private void InitializeItems()
    {
        for (int i = 0; i < itemDatas.Length; i++)
        {
            itemDatas[i].Initialize();
        }
    }
}


