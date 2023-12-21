using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] MenuController menuController = null; // Controller REF

    //----------BUTTONS-----------------------------------
    [SerializeField] Button gameplayButton = null;
    [SerializeField] Button displayButton = null;
    [SerializeField] Button audioButton = null;
    [SerializeField] Button graphicsButton = null;
    [SerializeField] Button returnButton = null;
    public Button ReturnButton => returnButton; // accessor for Controller Logic
    
    //----------MENU PANELS-------------------------------
    [SerializeField] GameObject gameplayMenu = null;
    [SerializeField] GameObject displayMenu = null;
    [SerializeField] GameObject audioMenu = null;
    [SerializeField] GameObject graphicsMenu = null;
    [SerializeField] GameObject activeMenu = null;
    
    void Start()
    {
        InitOptionsMenu();
    }

    void Update()
    {
        
    }

    void InitOptionsMenu() // Onclick events with lambda to add corresponding menu as argument
    {
        gameplayButton.onClick.AddListener(() => 
        { 
            SwitchMenu(gameplayMenu); 
        });

        displayButton.onClick.AddListener(() => 
        { 
            SwitchMenu(displayMenu); 
        });

        audioButton.onClick.AddListener(() => 
        { 
            SwitchMenu(audioMenu); 
        });

        graphicsButton.onClick.AddListener(() => 
        { 
            SwitchMenu(graphicsMenu); 
        });
    }
    
    void SwitchMenu(GameObject _toShow) // Simple switch with target menu as argument
    {
        if(activeMenu)
        activeMenu.SetActive(false);

        _toShow.SetActive(true);
        activeMenu = _toShow;
    }

    
}
