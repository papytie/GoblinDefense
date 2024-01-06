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
    [SerializeField] int selectedCameraIndex = 0;
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
        previousCameraButton.onClick.AddListener(DecreaseIndex);
        nextCameraButton.onClick.AddListener(IncreaseIndex);

        ChangeCameraPriority(); //change Camera focus
    }

    void IncreaseIndex()
    {
        selectedCameraIndex++;
        ChangeCameraPriority();
    }

    void DecreaseIndex()
    {
        selectedCameraIndex--;
        ChangeCameraPriority();
    }

    void ChangeCameraPriority()
    {
        if (selectedCameraIndex >= allCameras.Count) selectedCameraIndex = 0;
        if (selectedCameraIndex < 0) selectedCameraIndex = allCameras.Count - 1;
        foreach (CinemachineVirtualCamera _cam in allCameras)
        {
            _cam.Priority = 1;
        }
        allCameras[selectedCameraIndex].Priority = 10;
    }


}
