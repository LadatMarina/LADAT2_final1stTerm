using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager 
{
    public enum Sound
    {
        ButtonClick,
        ButtonOver,
        SnakeDie,
        SnakeEat,
        SnakeMove,
        WrongDirection,
        BeepSound,
        NewHighScoreSound,
        WinnerSound
    }

    private static GameObject soundManagerGameObject;
    public static AudioSource audioSource;

    public static void CreateSoundManagerGameObject()
    {
        if (soundManagerGameObject == null)
        {
            soundManagerGameObject = new GameObject("Sound Manager");
            audioSource = soundManagerGameObject.AddComponent<AudioSource>();
            
        }
        else
        {
            Debug.LogError("Sound Manager already exists");
        }
    }
    
    public static void PlaySound(Sound sound)
    {
        audioSource.PlayOneShot(GetAudioClipFromSound(sound));
    }

    private static AudioClip GetAudioClipFromSound(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClipsArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found");
        return null;
    }

    public static int SaveinitialPitch()
    {
        return (int)audioSource.pitch;
    }

    public static void ModifyPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }
}
