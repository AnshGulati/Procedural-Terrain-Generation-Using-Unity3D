using UnityEngine;
using UnityEngine.UI; // For UI Text

public class CoinPickup : MonoBehaviour
{
    public Text coinText; // Assign your UI Text in Inspector
    public int coins = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get a random multiple of 10 between 10 and 1000
            int randomCoins = Random.Range(1, 101) * 10;

            coins += randomCoins;

            // Update the UI text
            coinText.text = "Coins: " + coins;

            // Destroy the coin object
            Destroy(gameObject);
        }
    }
}