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

    private float displayedProgress = 0f; // what we show on UI
    private bool isDelayStarted = false;

    public void StartGame()
    {

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
                //float progress = Mathf.Clamp01(loadSceneOne.progress / 0.9f);
                //progressImage.fillAmount = progress;
                //loadingText.text = Mathf.RoundToInt(progress * 100f) + "%";

                float targetProgress = Mathf.Clamp01(loadSceneOne.progress / 0.9f);
                displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.3f);
                progressImage.fillAmount = displayedProgress;
                loadingText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";

                // When fully loaded (progress ~0.9), trigger the delay coroutine once
                if (displayedProgress >= 1f && !isDelayStarted)
                {
                    StartCoroutine(ActivateSceneAfterDelay(5f)); // delay
                }
            }
        }
    }

    private IEnumerator ActivateSceneAfterDelay(float delay)
    {
        isDelayStarted = true;
        yield return new WaitForSeconds(delay);
        loadSceneOne.allowSceneActivation = true;
    }

}