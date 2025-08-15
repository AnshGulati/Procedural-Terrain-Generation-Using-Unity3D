using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    public GameObject homeScreen;
    public GameObject controlScreen;
    public GameObject loadingScreen;

    private void Start()
    {
        homeScreen.SetActive(true);
        controlScreen.SetActive(false);
        loadingScreen.SetActive(false);
    }

    public void ShowControls()
    {
        homeScreen.SetActive(false);
        controlScreen.SetActive(true);
    }

    public void HideControls()
    {
        homeScreen.SetActive(true);
        controlScreen.SetActive(false);
    }
}
