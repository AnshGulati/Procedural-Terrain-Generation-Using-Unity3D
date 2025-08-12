using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIScreen : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        controlsMenu.SetActive(false);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ControlScreen()
    {
        controlsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 0;
    }

    public void RevertControlScreen()
    {
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void HomeScreen()
    {
        SceneManager.LoadScene(0);
    }
}
