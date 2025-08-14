using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shelter : MonoBehaviour
{
    public int currentHP;
    public int maxHP;
    public Image healthBar;
    public GameObject healthBarParent;
    public GameObject[] shelterModels; // 0 = Wooden, 1 = Stone, 2 = Metal
    public Material dayNightMaterial; // Same material as in DayCounter
    public float nightThreshold = 0.5f; // When blendValue >= threshold → it's night

    private int currentTier = 0;
    private float blendValue;

    private void Start()
    {
        SetTier(0); // Start with Wooden
        UpdateHealthBar();
        healthBarParent.SetActive(false);
    }

    private void Update()
    {
        // Get current BlendValue from the day-night material
        blendValue = dayNightMaterial.GetFloat("_BlendValue");

        // Show health bar only at night
        if (blendValue >= nightThreshold)
            healthBarParent.SetActive(true);
        else
            healthBarParent.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            GameOver();
        }
        UpdateHealthBar();
    }

    public void Repair()
    {
        currentHP = maxHP;
        UpdateHealthBar();
    }

    public void Upgrade()
    {
        if (currentTier < shelterModels.Length - 1)
        {
            currentTier++;
            SetTier(currentTier);
        }
    }

    private void SetTier(int tier)
    {
        for (int i = 0; i < shelterModels.Length; i++)
            shelterModels[i].SetActive(i == tier);

        switch (tier)
        {
            case 0: maxHP = 100; break; // Wooden
            case 1: maxHP = 200; break; // Stone
            case 2: maxHP = 400; break; // Metal
        }
        currentHP = maxHP;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)currentHP / maxHP;
    }

    private void GameOver()
    {
        Debug.Log("Game Over - Shelter Destroyed!");
        SceneManager.LoadScene(2); // Game Over scene
    }
}