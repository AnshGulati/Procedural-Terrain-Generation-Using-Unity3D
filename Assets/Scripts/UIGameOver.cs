using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI daysCounterText;
    public TextMeshProUGUI scoreCounterText;

    void Start()
    {
        enemiesKilledText.text = PlayerController.enemiesKilled.ToString();
        daysCounterText.text = DayCounter.dayCount.ToString();
        int totalScore = (int)((DayCounter.dayCount * DayCounter.dayCount * 20) + (PlayerController.enemiesKilled * 15));
        scoreCounterText.text = totalScore.ToString();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
