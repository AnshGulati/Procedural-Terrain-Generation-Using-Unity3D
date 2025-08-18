using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int Coins { get; private set; }
    public int Wood { get; private set; }
    public int Stone { get; private set; }

    [Header("UI References")]
    public List<TextMeshProUGUI> coinTexts;
    public List<TextMeshProUGUI> woodTexts;
    public List<TextMeshProUGUI> stoneTexts;

    [Header("Popup UI References")]
    public GameObject coinPopup;
    public GameObject woodLogPopup;
    public GameObject stonePopup;


    public float popupDuration = 2f;

    [Header("Sound")]
    // FIX: Changed to AudioSource, so we need to reference an AudioClip
    public AudioSource audioSource;
    public AudioClip collectSound;

    private UIManager uiManager;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        uiManager=FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("ResourceManager could not find the UIManager in the scene!");
        }
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateUI();
        PlayCollectSound();
        uiManager?.ShowCoinPopup();
    }

    public void AddWood(int amount)
    {
        Wood += amount;
        UpdateUI();
        PlayCollectSound();
        uiManager?.ShowWoodPopup();
    }

    public void AddStone(int amount)
    {
        Stone += amount;
        UpdateUI();
        PlayCollectSound();
        uiManager?.ShowStonePopup();
    }

    public void SpendResources(int coins, int wood, int stone)
    {
        Coins -= coins;
        Wood -= wood;
        Stone -= stone;
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var coinText in coinTexts)
            if (coinText != null) coinText.text = Coins.ToString();

        foreach (var woodText in woodTexts)
            if (woodText != null) woodText.text = Wood.ToString();

        foreach (var stoneText in stoneTexts)
            if (stoneText != null) stoneText.text = Stone.ToString();
    }

    private void PlayCollectSound()
    {
        if (audioSource != null && collectSound != null)
        {
            audioSource.clip = collectSound;
            audioSource.Play();
        }
    }
}
