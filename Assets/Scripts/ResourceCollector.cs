using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    public enum ResourceType { Coin, Wood, Stone }
    public ResourceType resourceType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int value = Random.Range(1, 20) * 10; // 10, 20, ... 200

            switch (resourceType)
            {
                case ResourceType.Coin:
                    ResourceManager.instance.AddCoins(value);
                    break;

                case ResourceType.Wood:
                    ResourceManager.instance.AddWood(value);
                    break;

                case ResourceType.Stone:
                    ResourceManager.instance.AddStone(value);
                    break;
            }

            Destroy(gameObject); // Remove collectible
        }
    }
}