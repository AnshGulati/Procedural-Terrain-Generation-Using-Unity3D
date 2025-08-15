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
        healthBarParent.SetActive(blendValue >= nightThreshold);
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
        // Repair cost: 100 coins + 50 wood + 50 stone
        if (ResourceManager.instance.Coins >= 100 &&
            ResourceManager.instance.Wood >= 50 &&
            ResourceManager.instance.Stone >= 50)
        {
            ResourceManager.instance.SpendResources(100, 50, 50);
            currentHP = maxHP;
            UpdateHealthBar();
            Debug.Log("Shelter repaired!");
        }
        else
        {
            Debug.Log("Not enough resources to repair shelter!");
        }
    }

    public void Upgrade()
    {
        // Tier 0 → Tier 1: 200 coins + 100 wood + 150 stone
        if (currentTier == 0 &&
            ResourceManager.instance.Coins >= 200 &&
            ResourceManager.instance.Wood >= 100 &&
            ResourceManager.instance.Stone >= 150)
        {
            ResourceManager.instance.SpendResources(200, 100, 150);
            currentTier++;
            SetTier(currentTier);
            Debug.Log("Shelter upgraded to Stone!");
        }
        // Tier 1 → Tier 2: 500 coins + 200 wood + 250 stone
        else if (currentTier == 1 &&
                 ResourceManager.instance.Coins >= 500 &&
                 ResourceManager.instance.Wood >= 200 &&
                 ResourceManager.instance.Stone >= 250)
        {
            ResourceManager.instance.SpendResources(500, 200, 250);
            currentTier++;
            SetTier(currentTier);
            Debug.Log("Shelter upgraded to Metal!");
        }
        else
        {
            Debug.Log("Not enough resources to upgrade!");
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