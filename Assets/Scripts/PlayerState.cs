using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    [Header("Player Stats")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;

    public float currentCalories = 100f;
    public float maxCalories = 100f;

    public float currentHydrationPercent = 100f;
    public float maxHydrationPercent = 100f;

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

    public void setHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    public void setCalories(float value)
    {
        currentCalories = Mathf.Clamp(value, 0, maxCalories);
    }

    public void setHydration(float value)
    {
        currentHydrationPercent = Mathf.Clamp(value, 0, maxHydrationPercent);
    }
}
