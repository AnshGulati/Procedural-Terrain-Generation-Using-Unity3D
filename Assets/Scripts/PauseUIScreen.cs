//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class PauseUIScreen : MonoBehaviour
//{
//    public GameObject pauseMenu;
//    public GameObject controlsMenu;

//    private void Start()
//    {
//        pauseMenu.SetActive(false);
//        controlsMenu.SetActive(false);
//    }

//    public void Pause()
//    {
//        pauseMenu.SetActive(true);
//        controlsMenu.SetActive(false);
//        Time.timeScale = 0;
//    }

//    public void Resume()
//    {
//        pauseMenu.SetActive(false);
//        controlsMenu.SetActive(false);
//        Time.timeScale = 1;
//    }

//    public void ControlScreen()
//    {
//        controlsMenu.SetActive(true);
//        pauseMenu.SetActive(false);
//        Time.timeScale = 0;
//    }

//    public void RevertControlScreen()
//    {
//        controlsMenu.SetActive(false);
//        pauseMenu.SetActive(true);
//        Time.timeScale = 0;
//    }

//    public void HomeScreen()
//    {
//        SceneManager.LoadScene(0);
//    }
//}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class PauseUIScreen : MonoBehaviour
//{
//    public GameObject pauseMenu;
//    public GameObject controlsMenu;

//    private bool isPaused = false;

//    private void Start()
//    {
//        pauseMenu.SetActive(false);
//        controlsMenu.SetActive(false);
//    }

//    private void Update()
//    {
//        // Listen for Escape key press
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (isPaused)
//            {
//                Resume();
//            }
//            else
//            {
//                Pause();
//            }
//        }
//    }

//    public void Pause()
//    {
//        pauseMenu.SetActive(true);
//        controlsMenu.SetActive(false);
//        Time.timeScale = 0;
//        isPaused = true;
//    }

//    public void Resume()
//    {
//        pauseMenu.SetActive(false);
//        controlsMenu.SetActive(false);
//        Time.timeScale = 1;
//        isPaused = false;
//    }

//    public void ControlScreen()
//    {
//        controlsMenu.SetActive(true);
//        pauseMenu.SetActive(false);
//        Time.timeScale = 0;
//    }

//    public void RevertControlScreen()
//    {
//        controlsMenu.SetActive(false);
//        pauseMenu.SetActive(true);
//        Time.timeScale = 0;
//    }

//    public void HomeScreen()
//    {
//        Time.timeScale = 1; // Reset before loading menu
//        SceneManager.LoadScene(0);
//    }
//}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIScreen : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public Shelter shelter;

    private bool isPaused = false;

    private void Start()
    {
        // Initially hide all menus
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);

        // Lock and hide the cursor at the start of the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Listen for Escape key press to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && !shelter.canAccessbuilder)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Listen for 'R' key press to resume the game
        if (Input.GetKeyDown(KeyCode.R) && isPaused)
        {
            Resume();
        }

        // Listen for 'C' key press to open the controls screen
        if (Input.GetKeyDown(KeyCode.C) && isPaused)
        {
            ControlScreen();
        }

        // Listen for 'Tab' key press to return to the home screen
        if (Input.GetKeyDown(KeyCode.Tab) && isPaused)
        {
            HomeScreen();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        controlsMenu.SetActive(false);
        Time.timeScale = 0;
        isPaused = true;

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ControlScreen()
    {
        controlsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        // Time.timeScale remains 0
    }

    public void RevertControlScreen()
    {
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        // Time.timeScale remains 0
    }

    public void HomeScreen()
    {
        Time.timeScale = 1; // Ensure game is unpaused before loading a new scene
        SceneManager.LoadScene(0);
    }
}