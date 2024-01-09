using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Spawner spawner;
    [SerializeField] GameObject playerHUD;
    [SerializeField] GameObject gameOverPopup;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject victoryPopup;

    [Header("PlayerUI")]
    [SerializeField] Slider playerHPBar = null;
    //[SerializeField] TextMeshProUGUI playerHPText = null;
    [SerializeField] TextMeshProUGUI playerMoneyText = null;

    [Header("WaveUI")]
    [SerializeField] Slider waveTimerBar = null;
    [SerializeField] TextMeshProUGUI waveTimerText = null;
    //[SerializeField] TextMeshProUGUI waveTimerText = null;

    [Header("NavUI")]
    [SerializeField] int selectedCameraIndex = 0;
    [SerializeField] Button previousCameraButton = null;
    [SerializeField] Button nextCameraButton = null;
    [SerializeField] Button topCameraButton = null;
    [SerializeField] CinemachineVirtualCamera topCamera = null;
    [SerializeField] List<CinemachineVirtualCamera> allCameras = new();

    [Header("MenuUI")]
    [SerializeField] Button openMenuButton = null;
    [SerializeField] Button menuResumeButton = null;
    [SerializeField] Button menuRetryButton = null;
    [SerializeField] Button menuQuitButton = null;
    [SerializeField] Button menuBackButton = null;

    [Header("Game Over Popup")]
    [SerializeField] Button retryButton = null;
    [SerializeField] Button titleButton = null;

    [Header("Victory Popup")]
    [SerializeField] Button victoryTitleButton = null;
    [SerializeField] float maxCarouselTime = 5;

    float currentTime = 0;

    private void Start()
    {
        InitGameHUD();
    }

    private void Update()
    {
        if (PlayerStats.Instance.GameIsOver || spawner.GameIsWin)
        {
            CarouselTimer();
            return;
        }

        playerHPBar.value = ConvertBarValueToFloat(PlayerStats.Instance.PlayerCurrentHealth,PlayerStats.Instance.PlayerBaseHealth);
        playerMoneyText.text = PlayerStats.Instance.PlayerCurrentMoney.ToString() + "$";
        waveTimerBar.value = 1 - (spawner.CurrentWaveTime / spawner.CurrentWaveTotalTime);
        waveTimerText.text = !spawner.IsWaveSpawning ? "Be Prepared..." : spawner.UIWaveIndex;
        
    }

    void InitGameHUD()
    {
        previousCameraButton.onClick.AddListener(DecreaseIndex);
        nextCameraButton.onClick.AddListener(IncreaseIndex);
        topCameraButton.onClick.AddListener(TopCameraPriority);
        openMenuButton.onClick.AddListener(ShowPauseMenu);

        menuBackButton.onClick.AddListener(ReturnToTitle);
        menuQuitButton.onClick.AddListener(QuitGame);
        menuResumeButton.onClick.AddListener(ResumeGame);
        menuRetryButton.onClick.AddListener(ResetLevel);
        victoryTitleButton.onClick.AddListener(ReturnToTitle);

        retryButton.onClick.AddListener(ResetLevel);
        titleButton.onClick.AddListener(ReturnToTitle);

        PlayerStats.Instance.OnGameOver += () =>
        {
            gameOverPopup.SetActive(true);
            playerHUD.SetActive(false);
        };

        spawner.OnLevelCleared += () =>
        {
            victoryPopup.SetActive(true);
        };

        ChangeCameraPriority(); //change Camera focus
    }

    float ConvertBarValueToFloat(int _currentInt, int _maxInt)
    {
        float _currentFloat = _currentInt;
        float _maxFloat = _maxInt;
        return _currentFloat / _maxFloat;
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
        topCamera.Priority = 1;
        if (selectedCameraIndex >= allCameras.Count) selectedCameraIndex = 0;
        if (selectedCameraIndex < 0) selectedCameraIndex = allCameras.Count - 1;
        foreach (CinemachineVirtualCamera _cam in allCameras)
        {
            _cam.Priority = 1;
        }
        allCameras[selectedCameraIndex].Priority = 10;
    }

    void TopCameraPriority()
    {
        foreach (CinemachineVirtualCamera _cam in allCameras)
        {
            _cam.Priority = 1;
        }
        topCamera.Priority = 10;
    }

    void CarouselTimer()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxCarouselTime)
        {
            currentTime = 0;
            selectedCameraIndex++;
            ChangeCameraPriority();
        }
    }

    void ResetLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameLevel");
    }

    void ReturnToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("OpenMenuThriller");
    }

    void ShowPauseMenu()
    {
        Time.timeScale = 0;
        pauseMenu.gameObject.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.gameObject.SetActive(false);
    }

    void QuitGame()
    {
        if (!menuQuitButton) return;
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }

}
