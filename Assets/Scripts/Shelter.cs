using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shelter : MonoBehaviour
{
    public int currentHP;
    public int maxHP;
    public GameObject healthBarParent;
    public Slider healthBarSlider;
    public GameObject[] shelterModels; // 0 = Wooden, 1 = Stone, 2 = Metal

    private int currentTier = 0;

    public bool canAccessbuilder = false;
    public GameObject builderHall;
    public GameObject repairUI;
    public GameObject upgradeUI0;
    public GameObject upgradeUI1;
    public GameObject upgradeUI2;

    public DayNightSystem dayNightSystem;
    public TimeManager timeManager;

    private float displayedProgress = 1f;
    public float fillSpeed = 0.3f;
    private bool isHealthBarActivated = false;

    public PauseUIScreen pauseScript;

    private void Start()
    {
        SetTier(0); // Start with Wooden
        UpdateHealthBar();
        healthBarParent.SetActive(false);
        builderHall.SetActive(false);
        repairUI.SetActive(false);
        upgradeUI0.SetActive(false);
        upgradeUI1.SetActive(false);
        upgradeUI2.SetActive(false);
    }

    private void Update()
    {
        if (dayNightSystem.currentHour == 22 && timeManager.dayInGame >= 1 && !isHealthBarActivated)
        {
            healthBarParent.SetActive(true);
            isHealthBarActivated = true;
        }

        if (dayNightSystem.currentHour == 6)
        {
            healthBarParent.SetActive(false);
            isHealthBarActivated = false;
        }

        if (canAccessbuilder && Input.GetKeyDown(KeyCode.B))
        {
            Time.timeScale = 0;
            builderHall.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (canAccessbuilder && Input.GetKeyDown(KeyCode.N))
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        pauseScript.isBuilding = false;
        Time.timeScale = 1;
        builderHall.SetActive(false);
        repairUI.SetActive(false);
        upgradeUI0.SetActive(false);
        upgradeUI1.SetActive(false);
        upgradeUI2.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        canAccessbuilder = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canAccessbuilder = false;
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

    public void RepairUI()
    {
        builderHall.SetActive(false);
        repairUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        ResumeGame();
    }

    public void UpgradeUI()
    {
        builderHall.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (currentTier == 0)
        {
            upgradeUI0.SetActive(true);
        }
        else if (currentTier == 1)
        {
            upgradeUI1.SetActive(true);
        }
        else
        {
            upgradeUI2.SetActive(true);
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
        // Tier 1 → Tier 2: 300 coins + 200 wood + 250 stone
        else if (currentTier == 1 &&
                 ResourceManager.instance.Coins >= 300 &&
                 ResourceManager.instance.Wood >= 200 &&
                 ResourceManager.instance.Stone >= 250)
        {
            ResourceManager.instance.SpendResources(300, 200, 250);
            currentTier++;
            SetTier(currentTier);
            Debug.Log("Shelter upgraded to Metal!");
        }
        else
        {
            Debug.Log("Not enough resources to upgrade!");
        }

        ResumeGame();
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
        healthBarSlider.value = (float)currentHP / maxHP;
    }

    private void GameOver()
    {
        Debug.Log("Game Over - Shelter Destroyed!");
        SceneManager.LoadScene(2); // Game Over scene
    }
}