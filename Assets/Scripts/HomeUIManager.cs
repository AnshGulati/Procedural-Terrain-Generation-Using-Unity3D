using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUIManager : MonoBehaviour
{
    public GameObject homeScreen;
    public GameObject controlScreen;

    private void Start()
    {
        homeScreen.SetActive(true);
        controlScreen.SetActive(false);
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
