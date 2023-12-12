using System;
using UnityEngine.SceneManagement;

public static class Loader
{
    // Variable que guarda una función sin inputs ni output
    private static Action loaderCallbackAction;
    
    // Lista de nuestras escenas
    public enum Scene
    {
        Game,
        LoadingScene,
        MainMenu
    }

    public static void Load(Scene scene)
    {
        // Asignas en loaderCallbackAction una función que no recibe parámetros y ejecuta la línea 25
        loaderCallbackAction = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };
        // Llamamos a la escena de carga
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        if (loaderCallbackAction != null)
        {
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
}
