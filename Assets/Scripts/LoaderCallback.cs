using UnityEngine;
using System.Collections;

public class LoaderCallback : MonoBehaviour
{
    private bool firstUpdate = true;
    public GameObject snakeHead;
    private int initialPitch;

    private void Awake()
    {
        SoundManager.CreateSoundManagerGameObject();
        initialPitch = SoundManager.SaveinitialPitch();        
    }
    private void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            StartCoroutine(WaitingTime());
        }
    }

    private IEnumerator WaitingTime()
    {
        
        for (float i = -8; i < 9; i++)
        {
            snakeHead.transform.position = new Vector3(i, 0, 0);
            SoundManager.ModifyPitch((i+9) * 0.1f);
            SoundManager.PlaySound(SoundManager.Sound.BeepSound);
            //Debug.Log("i");
            yield return new WaitForSeconds(.2f);
        }
        SoundManager.audioSource.pitch = initialPitch;
        Loader.LoaderCallback();
    }
}
