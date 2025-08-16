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

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        UpdateUI();
    }

    public void AddWood(int amount)
    {
        Wood += amount;
        UpdateUI();
    }

    public void AddStone(int amount)
    {
        Stone += amount;
        UpdateUI();
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
}