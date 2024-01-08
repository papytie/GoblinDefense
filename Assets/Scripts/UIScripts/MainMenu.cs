using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //[SerializeField] MenuController menuController = null; // Controller REF

    //-----------BUTTONS------------------------
    [SerializeField] Button startButton = null;
    [SerializeField] Button optionsButton = null;
    [SerializeField] Button quitButton = null;

    public Button OptionsButton => optionsButton; // Accessor for Controller Logic

    void Start()
    {
        InitMainMenu();
    }

    void Update()
    {
        
    }

    void InitMainMenu()
    {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        if (!startButton) return;
        SceneManager.LoadScene("GameLevel");
    }

    void QuitGame()
    {
        if(!quitButton) return;
        //EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
