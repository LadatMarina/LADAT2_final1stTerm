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
        playButton.onClick.AddListener(() => {
            SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
            Loader.Load(Loader.Scene.Game);
        });
        howToPlayButton.onClick.AddListener(() =>
        {
            SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
            ShowHowToPlayPanel();
        });
        
        quitButton.onClick.AddListener(()=> { 
            SoundManager.PlaySound(SoundManager.Sound.ButtonClick); 
            Application.Quit(); 
        });
        
        quitHowToPlayPanelButton.onClick.AddListener(()=> { 
            SoundManager.PlaySound(SoundManager.Sound.ButtonClick); 
            HideHowToPlayPanel(); 
        });
        
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
