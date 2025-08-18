using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIScreen : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public GameObject shelterUI;

    private bool isPaused = false;
    public bool isBuilding = false;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            for (int i = 0; i < shelterUI.transform.childCount; i++)
            {
                Transform child = shelterUI.transform.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    isBuilding = true;
                    break; // stop checking once one active child is found
                }
            }

            if (!isBuilding)
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
        if (!isBuilding)
        {
            pauseMenu.SetActive(true);
            controlsMenu.SetActive(false);
            Time.timeScale = 0;
            isPaused = true;

            // Unlock and show the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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
    }

    public void RevertControlScreen()
    {
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void HomeScreen()
    {
        Application.Quit();
    }
}