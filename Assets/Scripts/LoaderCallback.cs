using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool firstUpdate = true;

    private void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;
            Loader.LoaderCallback();
        }
    }
}
