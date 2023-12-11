using System;
using UnityEngine;

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

    private void Awake()
    {
        // Singleton
        if (Instance != null) 
        {
            Debug.LogError("There is more than one Instance");
        }

        Instance = this;
    }
}
