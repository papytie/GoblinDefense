using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSpotUI : UIWorldElement
{
    [Header("Tower Spot Settings")]

    [Header("UI References")]
    [SerializeField] GameObject buyPanel = null;
    [SerializeField] GameObject upgradePanel = null;
    [SerializeField] Button archerTowerButton = null;
    [SerializeField] Button mageTowerButton = null;
    [SerializeField] Button upgradeTowerButton = null;
    [SerializeField] TextMeshProUGUI archerPriceText = null;
    [SerializeField] TextMeshProUGUI magePriceText = null;
    [SerializeField] TextMeshProUGUI upgradePriceText = null;
    [SerializeField] TextMeshProUGUI towerNameText = null;

    [Header("Parameters")]
    [SerializeField] Tower archerTower = null;
    [SerializeField] Tower mageTower = null;
    [SerializeField] int archerBasePrice = 100;
    [SerializeField] int mageBasePrice = 200;
    [SerializeField] int towerMaxLevel = 3;

    Tower actualTower = null;
    int actualTowerLevel = 1;

    protected override void Start()
    {
        base.Start();
        InitTowerSpot();
    }

    protected override void Update()
    {
        base.Update();
    }

    void InitTowerSpot()
    {
        archerTowerButton.onClick.AddListener(BuyArcherTower);
        mageTowerButton.onClick.AddListener(BuyMageTower);
        upgradeTowerButton.onClick.AddListener(UpgradeTower);
        archerPriceText.text = archerBasePrice + "$";
        magePriceText.text = mageBasePrice + "$";
        upgradePriceText.text = mageBasePrice + "$";

        PlayerStats.Instance.OnGameOver += () => canvas.gameObject.SetActive(false);
    }

    void BuyArcherTower()
    {
        NewTower(archerTower);
    }

    void BuyMageTower()
    {
        NewTower(mageTower);
    }

    void NewTower(Tower _type)
    {
        if(PlayerStats.Instance.RemoveMoney(_type.BasePrice))
        {
            actualTower = Instantiate(_type, transform.position, transform.rotation, gameObject.transform);
            buyPanel.gameObject.SetActive(false);
            upgradePanel.SetActive(true);
            upgradePriceText.text = actualTower.UpgradePrice + "$";
            towerNameText.text = actualTower.TowerName + " lvl" + actualTowerLevel;
        }
    }

    void UpgradeTower()
    {
        if (PlayerStats.Instance.RemoveMoney(actualTower.UpgradePrice * actualTowerLevel))
        {
            actualTower.UpgradeTower(actualTowerLevel);
            actualTowerLevel++;
            if (actualTowerLevel == towerMaxLevel)
            {
                upgradePriceText.gameObject.SetActive(false);
                upgradeTowerButton.gameObject.SetActive(false);
            }
            towerNameText.text = actualTower.TowerName + " lvl" + actualTowerLevel;
            upgradePriceText.text = actualTower.UpgradePrice * actualTowerLevel + "$";
        }
    }

}
