using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    public int PlayerCurrentHealth => playerCurrentHealth;
    public int PlayerCurrentMoney => playerCurrentMoney;
    public int PlayerBaseHealth => playerBaseHealth;
    public bool GameIsOver => gameIsOver;

    public event Action OnGameOver = null;

    [Header("Player Settings")]

    [Header("Health")]
    [SerializeField] int playerBaseHealth = 10;
    [SerializeField] int playerMaxHealth = 20;
    [SerializeField] int playerCurrentHealth = 0;

    [Header("Money")]
    [SerializeField] int playerBaseMoney = 0;
    [SerializeField] int playerMaxMoney = 99;
    [SerializeField] int playerCurrentMoney = 0;

    bool gameIsOver = false;

    private void Start()
    {
        InitPlayerStats();
    }

    void InitPlayerStats()
    {
        playerCurrentHealth = playerBaseHealth;
        playerCurrentMoney = playerBaseMoney;
    } //Set current var

    public void AddHealth(int _heal)
    {
        playerCurrentHealth += _heal;
        playerCurrentHealth = playerCurrentHealth >= playerMaxHealth ? playerMaxHealth : playerCurrentHealth;
    } //Add heal to playerCurrentHealth clamped to playerMaxHealth

    public void AddMoney(int _gain)
    {
        playerCurrentMoney += _gain;
        playerCurrentMoney = playerCurrentMoney >= playerMaxMoney ? playerMaxMoney : playerCurrentMoney;
    } //Add gain to playerCurrentMoney clamped to playerMaxMoney

    public void RemoveHealth(int _damage)
    {
        playerCurrentHealth -= _damage;
        if (playerCurrentHealth <= 0) 
        {
            playerCurrentHealth = 0;
            gameIsOver = true;
            OnGameOver?.Invoke();
        }
    } //Remove health and set bool if GameIsOver

    public bool RemoveMoney(int _price)
    {
        if (_price > playerCurrentMoney) return false;
        else
            playerCurrentMoney -= _price;
        return true;
    } //return false if price is too high and proceed otherwise
}
