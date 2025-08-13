using UnityEngine;
using TMPro; // If you're using TextMeshPro instead of UI Text

public class DayCounter : MonoBehaviour
{
    public Material dayNightMaterial; // Assign your DayNightCycle material here
    public float dayThreshold = 0.1f; // Point where day starts
    public TMP_Text dayText; // Or use UnityEngine.UI.Text for normal Text UI

    private float blendValue;
    private float timeScale;
    public static int dayCount = 0;
    private bool wasNight = false;

    void Start()
    {
        blendValue = dayNightMaterial.GetFloat("_BlendValue");
        timeScale = dayNightMaterial.GetFloat("_TimeScale");
        UpdateDayText();
    }

    void Update()
    {
        // Update BlendValue manually from Time
        blendValue += Time.deltaTime * timeScale;
        if (blendValue > 1f) blendValue -= 1f; // Wrap around
        dayNightMaterial.SetFloat("_BlendValue", blendValue);

        // Detect transition from night to day
        if (blendValue < dayThreshold && wasNight)
        {
            dayCount++;
            UpdateDayText();
            wasNight = false;
        }
        else if (blendValue >= 0.5f) // Night phase
        {
            wasNight = true;
        }
    }

    void UpdateDayText()
    {
        if (dayText != null)
        {
            dayText.text = dayCount.ToString();
        }
    }
}
