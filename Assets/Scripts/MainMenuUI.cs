using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button howToPlayButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private Button quitHowToPlayPanelButton;

    [SerializeField] private GameObject howToPlayPanel;

    private void Awake()
    {
        playButton.onClick.AddListener(() => {Loader.Load(Loader.Scene.Game);});
        howToPlayButton.onClick.AddListener(ShowHowToPlayPanel);
        quitButton.onClick.AddListener(Application.Quit);
        
        quitHowToPlayPanelButton.onClick.AddListener(HideHowToPlayPanel);
        
        HideHowToPlayPanel();
        
        SoundManager.CreateSoundManagerGameObject();
    }

    private void ShowHowToPlayPanel()
    {
        howToPlayPanel.SetActive(true);
    }
    
    private void HideHowToPlayPanel()
    {
        howToPlayPanel.SetActive(false);
    }
}
