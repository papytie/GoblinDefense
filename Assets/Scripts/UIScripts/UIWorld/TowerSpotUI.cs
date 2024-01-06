using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerSpotUI : UIWorldElement
{
    [Header("Tower Spot Settings")]

    [Header("References")]
    [SerializeField] Button newTowerButton = null;
    [SerializeField] Button upgradeTowerButton = null;
    [SerializeField] TextMeshProUGUI priceText = null;

    [Header("Parameters")]
    [SerializeField] Tower baseTower = null;
    [SerializeField] Tower upgradedTower = null;
    [SerializeField] int towerPrice = 100;
    [SerializeField] int upgradePrice = 250;

    Tower actualTower = null;

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
        newTowerButton.onClick.AddListener(BuyNewTower);
        upgradeTowerButton.onClick.AddListener(UpgradeTower);
        priceText.text = towerPrice + "$";
    }

    void BuyNewTower()
    {
        if(PlayerStats.Instance.RemoveMoney(towerPrice))
        {
            actualTower = Instantiate(baseTower, transform.position, transform.rotation, gameObject.transform);
            newTowerButton.gameObject.SetActive(false);
            upgradeTowerButton.gameObject.SetActive(true);
            priceText.text = upgradePrice + "$";

        }

    }

    void UpgradeTower()
    {
        if (PlayerStats.Instance.RemoveMoney(upgradePrice))
        {
            actualTower.DestroyTower();
            actualTower = null;
            actualTower = Instantiate(upgradedTower, transform.position, transform.rotation, gameObject.transform);
            upgradeTowerButton.gameObject.SetActive(false);
        }
    }

}
