using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaidManager : MonoBehaviour
{
    public GameObject raiderPrefab;
    public Transform[] spawnPoints;
    public DayNightSystem dayNightSystem;
    public TimeManager timeManager;

    // Public variable for the spawning particle effect
    public GameObject spawnEffectPrefab;
    // Public list to hold the standing points
    public List<Transform> standingPoints;

    public int numberOfRaidersToSpawn = 5;

    // NEW: Public variable for the spawning sound effect
    public AudioClip spawnSound;
    private AudioSource audioSource;

    // NEW: Public variables for the end-of-raid burning effect
    public GameObject burnEffectPrefab;
    public float burnDeathDelay = 2.0f;

    // NEW: Time delay between each raider spawn
    public float spawnInterval = 2.5f;

    private List<GameObject> activeRaiders = new List<GameObject>();
    private List<GameObject> activeSpawnEffects = new List<GameObject>(); // NEW: List to track active spawn effects
    private bool isRaidActive = false;
    private bool dayEffectTriggered = false;

    public int dayToSpawn = 3;

    [Header("Popup UI References")]
    public GameObject RaidPopup;
    private UIManager uiManager;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("ResourceManager could not find the UIManager in the scene!");
        }
    }

    void Update()
    {
        if (dayNightSystem == null || timeManager == null)
        {
            Debug.LogError("DayNightSystem or TimeManager is not assigned in RaidManager!");
            return;
        }

        if (dayNightSystem.currentHour == 22 && timeManager.dayInGame >= dayToSpawn && !isRaidActive)
        {
            StartRaid();
        }

        // Check if the raid is active and the time is 6 AM
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
        dayEffectTriggered = false; // Reset the flag for the next raid
        Debug.Log("Raid started! Raiders are spawning...");
        uiManager?.ShowRaidPopup();

        StartCoroutine(SpawnRaidersCoroutine());
    }

    private IEnumerator SpawnRaidersCoroutine()
    {
        // Keep track of which spawn points have already been used for effects
        List<Transform> usedSpawnPointsForEffect = new List<Transform>();

        // Loop for the total number of raiders you want to spawn
        for (int i = 0; i < numberOfRaidersToSpawn; i++)
        {
            // Calculate the index of the spawn point to use this iteration
            int spawnPointIndex = i % spawnPoints.Length;
            Transform currentSpawnPoint = spawnPoints[spawnPointIndex];

            // Play spawn sound
            if (spawnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }

            // Instantiate the spawn effect only if this spawn point hasn't had one yet
            if (spawnEffectPrefab != null && !usedSpawnPointsForEffect.Contains(currentSpawnPoint))
            {
                GameObject newEffect = Instantiate(spawnEffectPrefab, currentSpawnPoint.position, currentSpawnPoint.rotation);
                activeSpawnEffects.Add(newEffect);
                usedSpawnPointsForEffect.Add(currentSpawnPoint);
            }

            // Instantiate the raider
            GameObject newRaider = Instantiate(raiderPrefab, currentSpawnPoint.position, currentSpawnPoint.rotation);
            activeRaiders.Add(newRaider);

            RaiderAI raiderAI = newRaider.GetComponent<RaiderAI>();
            if (raiderAI != null)
            {
                raiderAI.Initialize(standingPoints);
            }

            // Wait for the specified interval before spawning the next raider
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void EndRaid()
    {
        if (dayEffectTriggered) return; // Only run this once per raid
        dayEffectTriggered = true;

        isRaidActive = false;
        Debug.Log("Raid ended! Raiders are despawning...");

        // Destroy all active spawn effects
        foreach (GameObject spawnEffect in activeSpawnEffects)
        {
            if (spawnEffect != null)
            {
                Destroy(spawnEffect);
            }
        }
        activeSpawnEffects.Clear(); // Clear the list

        // For each surviving raider, start the burning effect and destroy them after a delay
        foreach (GameObject raider in activeRaiders)
        {
            if (raider != null)
            {
                // Instantiate the burn effect as a child of the raider
                if (burnEffectPrefab != null)
                {
                    GameObject burnEffect = Instantiate(burnEffectPrefab, raider.transform.position, Quaternion.identity, raider.transform);
                    Destroy(burnEffect, burnDeathDelay);
                }

                // Disable movement and colliders immediately
                NavMeshAgent agent = raider.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.isStopped = true;
                }

                Collider raiderCollider = raider.GetComponent<Collider>();
                if (raiderCollider != null)
                {
                    raiderCollider.enabled = false;
                }

                // Destroy the raider and all its children after the burnDeathDelay
                Destroy(raider, burnDeathDelay);
            }
        }
        activeRaiders.Clear();
    }
}