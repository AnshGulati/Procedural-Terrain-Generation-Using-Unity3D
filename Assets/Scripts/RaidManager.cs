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

    private List<GameObject> activeRaiders = new List<GameObject>();
    private List<GameObject> activeSpawnEffects = new List<GameObject>(); // NEW: List to track active spawn effects
    private bool isRaidActive = false;
    private bool dayEffectTriggered = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

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

        // Create a list to keep track of which spawn points have been used in this raid
        List<Transform> usedSpawnPoints = new List<Transform>();

        for (int i = 0; i < numberOfRaidersToSpawn; i++)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (spawnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }

            // Check if this spawn point already has an effect to avoid overlapping effects at the start
            if (spawnEffectPrefab != null && !usedSpawnPoints.Contains(randomSpawnPoint))
            {
                // Instantiate the spawn effect and store a reference
                GameObject newEffect = Instantiate(spawnEffectPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
                activeSpawnEffects.Add(newEffect);
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