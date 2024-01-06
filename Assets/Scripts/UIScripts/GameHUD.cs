using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Spawner spawner;
    [Header("PlayerUI")]
    [SerializeField] Slider playerHPBar = null;
    //[SerializeField] TextMeshProUGUI playerHPText = null;
    [SerializeField] TextMeshProUGUI playerMoneyText = null;
    [Header("WaveUI")]
    [SerializeField] Slider waveTimerBar = null;
    //[SerializeField] TextMeshProUGUI waveTimerText = null;
    [Header("NavUI")]
    [SerializeField] Button previousCameraButton = null;
    [SerializeField] Button nextCameraButton = null;
    [SerializeField] List<CinemachineVirtualCamera> allCameras = new();
    [Header("MenuUI")]
    [SerializeField] Button menuButton = null;

    private void Start()
    {
        InitGameHUD();
    }

    private void Update()
    {
        playerHPBar.value = ConvertBarValueToFloat(PlayerStats.Instance.PlayerCurrentHealth,PlayerStats.Instance.PlayerBaseHealth);
        playerMoneyText.text = PlayerStats.Instance.PlayerCurrentMoney.ToString() + "$";
        waveTimerBar.value = 1 - (spawner.CurrentWaveTime / spawner.CurrentWaveTotalTime);
    }

    float ConvertBarValueToFloat(int _currentInt, int _maxInt)
    {
        float _currentFloat = _currentInt;
        float _maxFloat = _maxInt;
        return _currentFloat / _maxFloat;
    }
    void InitGameHUD()
    {
        
    }

}
