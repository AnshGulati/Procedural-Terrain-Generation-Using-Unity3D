/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    public GameObject raiderPrefab;
    public Transform[] spawnPoints;
    public DayNightSystem dayNightSystem;
    public TimeManager timeManager;

    // New variable to control the number of raiders
    public int numberOfRaidersToSpawn = 5;

    private List<GameObject> activeRaiders = new List<GameObject>();
    private bool isRaidActive = false;

    void Update()
    {
        // Check for the night to start the raid
        if (dayNightSystem.currentHour == 22 && timeManager.dayInGame >= 1 && !isRaidActive)
        {
            StartRaid();
        }

        // Check for the morning to end the raid
        if (dayNightSystem.currentHour == 6 && isRaidActive)
        {
            EndRaid();
        }
    }

    private void StartRaid()
    {
        isRaidActive = true;
        Debug.Log("Raid started! Raiders are spawning...");

        // Spawn a specified number of raiders and assign them random spawn points
        for (int i = 0; i < numberOfRaidersToSpawn; i++)
        {
            // Pick a random spawn point from the array
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the raider at the chosen spawn point
            GameObject newRaider = Instantiate(raiderPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
            activeRaiders.Add(newRaider);
        }
    }

    private void EndRaid()
    {
        isRaidActive = false;
        Debug.Log("Raid ended! Raiders are despawning...");

        // Destroy all currently active raiders
        foreach (GameObject raider in activeRaiders)
        {
            if (raider != null)
            {
                Destroy(raider);
            }
        }
        activeRaiders.Clear();
    }
}*/

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    public GameObject raiderPrefab;
    public Transform[] spawnPoints;
    public DayNightSystem dayNightSystem;
    public TimeManager timeManager;

    // The number of raiders to spawn for the raid
    public int numberOfRaidersToSpawn = 5;

    private List<GameObject> activeRaiders = new List<GameObject>();
    private bool isRaidActive = false;

    void Update()
    {
        // Check for the night to start the raid
        if (dayNightSystem.currentHour == 22 && timeManager.dayInGame >= 1 && !isRaidActive)
        {
            StartRaid();
        }

        // Check for the morning to end the raid
        if (dayNightSystem.currentHour == 6 && isRaidActive)
        {
            EndRaid();
        }
    }

    private void StartRaid()
    {
        isRaidActive = true;
        Debug.Log("Raid started! Raiders are spawning...");

        // Spawn a specified number of raiders and assign them random spawn points
        for (int i = 0; i < numberOfRaidersToSpawn; i++)
        {
            // Pick a random spawn point from the array
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the raider at the chosen spawn point
            GameObject newRaider = Instantiate(raiderPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
            activeRaiders.Add(newRaider);
        }
    }

    private void EndRaid()
    {
        isRaidActive = false;
        Debug.Log("Raid ended! Raiders are despawning...");

        // Destroy all currently active raiders
        foreach (GameObject raider in activeRaiders)
        {
            if (raider != null)
            {
                Destroy(raider);
            }
        }
        activeRaiders.Clear();
    }
}
*/


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    public GameObject raiderPrefab;
    public Transform[] spawnPoints;
    public DayNightSystem dayNightSystem;
    public TimeManager timeManager;

    // Add this public variable for the particle effect prefab
    public GameObject spawnEffectPrefab;
    // Add this public list to hold the standing points
    public List<Transform> standingPoints;

    public int numberOfRaidersToSpawn = 5;

    private List<GameObject> activeRaiders = new List<GameObject>();
    // NEW: A list to keep track of the spawned particle effects
    private List<GameObject> activeSpawnEffects = new List<GameObject>();
    private bool isRaidActive = false;

    void Update()
    {
        if (dayNightSystem == null || timeManager == null)
        {
            Debug.LogError("DayNightSystem or TimeManager is not assigned in RaidManager!");
            return;
        }

        if (dayNightSystem.currentHour == 22 && timeManager.dayInGame >= 1 && !isRaidActive)
        {
            StartRaid();
        }

        if (dayNightSystem.currentHour == 6 && isRaidActive)
        {
            EndRaid();
        }
    }

    private void StartRaid()
    {
        if (raiderPrefab == null)
        {
            Debug.LogError("Raider Prefab is not assigned in RaidManager!");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points array is empty or null in RaidManager!");
            return;
        }

        isRaidActive = true;
        Debug.Log("Raid started! Raiders are spawning...");

        for (int i = 0; i < numberOfRaidersToSpawn; i++)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the raider first
            GameObject newRaider = Instantiate(raiderPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
            activeRaiders.Add(newRaider);

            RaiderAI raiderAI = newRaider.GetComponent<RaiderAI>();
            if (raiderAI != null)
            {
                raiderAI.Initialize(standingPoints);
            }

            // NEW: Instantiate the particle effect and parent it to the new raider
            if (spawnEffectPrefab != null)
            {
                GameObject newEffect = Instantiate(spawnEffectPrefab, newRaider.transform);
                activeSpawnEffects.Add(newEffect);
            }
        }
    }

    private void EndRaid()
    {
        isRaidActive = false;
        Debug.Log("Raid ended! Raiders are despawning...");

        // NEW: First, destroy the particle effects
        foreach (GameObject effect in activeSpawnEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeSpawnEffects.Clear();

        // Then, destroy the raiders
        foreach (GameObject raider in activeRaiders)
        {
            if (raider != null)
            {
                Destroy(raider);
            }
        }
        activeRaiders.Clear();
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    public GameObject raiderPrefab;
    public Transform[] spawnPoints;
    public DayNightSystem dayNightSystem;
    public TimeManager timeManager;

    // Public variable for the particle effect prefab
    public GameObject spawnEffectPrefab;
    // Public list to hold the standing points
    public List<Transform> standingPoints;

    public int numberOfRaidersToSpawn = 5;

    private List<GameObject> activeRaiders = new List<GameObject>();
    // NEW: A list to keep track of the spawned particle effects
    private List<GameObject> activeSpawnEffects = new List<GameObject>();
    private bool isRaidActive = false;

    void Update()
    {
        if (dayNightSystem == null || timeManager == null)
        {
            Debug.LogError("DayNightSystem or TimeManager is not assigned in RaidManager!");
            return;
        }

        if (dayNightSystem.currentHour == 22 && timeManager.dayInGame >= 1 && !isRaidActive)
        {
            StartRaid();
        }

        if (dayNightSystem.currentHour == 6 && isRaidActive)
        {
            EndRaid();
        }
    }

    private void StartRaid()
    {
        if (raiderPrefab == null)
        {
            Debug.LogError("Raider Prefab is not assigned in RaidManager!");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points array is empty or null in RaidManager!");
            return;
        }

        isRaidActive = true;
        Debug.Log("Raid started! Raiders are spawning...");

        // Create a list to keep track of which spawn points have been used in this raid
        List<Transform> usedSpawnPoints = new List<Transform>();

        for (int i = 0; i < numberOfRaidersToSpawn; i++)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // NEW LOGIC: Instantiate the particle effect once per spawn point
            if (spawnEffectPrefab != null && !usedSpawnPoints.Contains(randomSpawnPoint))
            {
                // Create the effect as a standalone object
                GameObject newEffect = Instantiate(spawnEffectPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
                // Add the effect to our tracking list
                activeSpawnEffects.Add(newEffect);
                // Mark this spawn point as used so we don't create multiple effects on it
                usedSpawnPoints.Add(randomSpawnPoint);
            }

            GameObject newRaider = Instantiate(raiderPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
            activeRaiders.Add(newRaider);

            RaiderAI raiderAI = newRaider.GetComponent<RaiderAI>();
            if (raiderAI != null)
            {
                raiderAI.Initialize(standingPoints);
            }
        }
    }

    private void EndRaid()
    {
        isRaidActive = false;
        Debug.Log("Raid ended! Raiders are despawning...");

        // First, destroy all remaining raiders
        foreach (GameObject raider in activeRaiders)
        {
            if (raider != null)
            {
                Destroy(raider);
            }
        }
        activeRaiders.Clear();

        // Second, destroy all the persistent particle effects
        foreach (GameObject effect in activeSpawnEffects)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeSpawnEffects.Clear();
    }
}
