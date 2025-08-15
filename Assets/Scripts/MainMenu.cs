using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Loading")]
    AsyncOperation loadSceneOne;
    public Image progressImage;
    bool sceneRequested = false;
    public GameObject loadingScreen;
    public GameObject mainScreen;
    public TextMeshProUGUI loadingText;

    public void StartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        loadSceneOne = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        sceneRequested = true;
        loadingScreen.SetActive(true);
        mainScreen.SetActive(false); 
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (sceneRequested)
        {
            if (loadSceneOne.isDone == false)
            {
                progressImage.fillAmount = loadSceneOne.progress;
                loadingText.text = loadSceneOne.progress * 100f + "%";
            }
        }
    }

}