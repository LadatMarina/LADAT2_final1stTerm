using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAssets : MonoBehaviour
{
    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
    
    public static GameAssets Instance { get; private set; }

    public Sprite snakeHeadSprite;
    public Sprite snakeBodySprite;
    public Sprite foodSprite;

    public SoundAudioClip[] soundAudioClipsArray;

    public GameObject[] arrowsArray;
    public GameObject arrowParent;
    public GameObject winnerPanel;

    private void Awake()
    {
        // Singleton
        if (Instance != null) 
        {
            Debug.LogError("There is more than one Instance");
        }

        Instance = this;
        Scene actualScene = SceneManager.GetActiveScene();

        if (actualScene.buildIndex == 1) //if the scene is the game scene;
        {
            winnerPanel.gameObject.SetActive(false); //inicialitze winner panel
        }
    }
}
