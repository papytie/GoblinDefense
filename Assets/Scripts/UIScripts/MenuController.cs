using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] MainMenu mainMenuRef = null;
    [SerializeField] OptionsMenu optionsMenuRef = null;
    public MainMenu MainMenuRef => mainMenuRef;
    public OptionsMenu OptionsMenuRef => optionsMenuRef;

    void Start()
    {
        mainMenuRef.OptionsButton.onClick.AddListener(SwitchMenuVisibility);
        optionsMenuRef.ReturnButton.onClick.AddListener(SwitchMenuVisibility);
    }

    void Update()
    {
        
    }

    public void SwitchMenuVisibility() // Switch Visibility between MainMenu/OptionsMenu
    {
        if (!mainMenuRef || !optionsMenuRef) return;
        mainMenuRef.gameObject.SetActive(!mainMenuRef.gameObject.activeInHierarchy);
        optionsMenuRef.gameObject.SetActive(!optionsMenuRef.gameObject.activeInHierarchy);
    }
}
