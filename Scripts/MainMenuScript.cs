using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LaunchGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LaunchPlayback()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Debug.Log("Application quit would work outside of editor.");
        Application.Quit();
    }
}
