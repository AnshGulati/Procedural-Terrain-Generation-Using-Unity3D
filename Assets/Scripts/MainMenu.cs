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
        loadSceneOne.allowSceneActivation = false;
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
                //progressImage.fillAmount = loadSceneOne.progress;
                //loadingText.text = loadSceneOne.progress * 100f + "%";

                float progress = Mathf.Clamp01(loadSceneOne.progress / 0.9f);
                progressImage.fillAmount = progress;
                loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";

                // When fully loaded (progress ~0.9), trigger the delay coroutine once
                if (progress >= 1f && !isDelayStarted)
                {
                    StartCoroutine(ActivateSceneAfterDelay(10f)); // 10-second delay
                }
            }
        }
    }

    private bool isDelayStarted = false;

    private IEnumerator ActivateSceneAfterDelay(float delay)
    {
        isDelayStarted = true;
        yield return new WaitForSeconds(delay);
        loadSceneOne.allowSceneActivation = true;
    }

}