using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public int dayInGame = 1;
    public TextMeshProUGUI dayUI;

    private void Start()
    {
        dayUI.text = dayInGame.ToString();
    }

    public void TriggerNextDay()
    {
        dayInGame++;
        dayUI.text = dayInGame.ToString();
    }
}
