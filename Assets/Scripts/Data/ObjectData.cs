using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectData : UpdatableData
{
    [Header("Object Spawning")]
    public PlaceableObject[] placeableObjects;

    // The maxSlope field was moved into PlacementRules in the previous step, so it's commented out here
    // as per your provided "old script". It's now a per-object rule.
    // [Header("Placement Rules")]
    // [Tooltip("The maximum slope in degrees that objects can spawn on.")]
    // public float maxSlope = 35f;


    [Tooltip("The minimum distance between any two spawned objects.")]
    public float minObjectRadius = 10f;

    [Header("Noise Settings")]
    [Tooltip("A separate seed for object placement to vary it independently of terrain shape.")]
    public int objectSeed;
    [Tooltip("Affects the size of clusters. Larger values = smaller, more frequent clusters.")]
    public float noiseScale = 50f;
    [Tooltip("Offsets the noise pattern.")]
    public Vector2 noiseOffset;


    [System.Serializable]
    public struct PlaceableObject
    {
        public string name;
        public GameObject prefab;
        [Range(0, 1)]
        [Tooltip("The noise value required to spawn this. Higher is rarer.")]
        public float threshold;
        [Tooltip("Rules for where this specific object can spawn.")]
        public PlacementRules rules;

        [Tooltip("The indices of the texture layers this object can spawn on (from TextureData).")]
        public int[] spawnableTextureIndices;
    }

    [System.Serializable]
    public struct PlacementRules
    {
        [Tooltip("Minimum world height.")]
        public float minHeight;
        [Tooltip("Maximum world height.")]
        public float maxHeight;

        [Tooltip("The maximum slope in degrees that this specific object can spawn on.")]
        public float maxSlope; // Removed the direct '= 35f' initialization here

        // EXPLICIT CONSTRUCTOR ADDED TO INITIALIZE FIELDS
        public PlacementRules(float minHeight, float maxHeight, float maxSlope)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
            this.maxSlope = maxSlope;
        }

        // You might consider adding a parameterless constructor if you want default values to apply
        // without always explicitly passing them when creating PlacementRules.
        // If you add this, the fields won't need to be initialized in the above constructor.
        // public PlacementRules()
        // {
        //     minHeight = 0f;
        //     maxHeight = 1f;
        //     maxSlope = 35f; // Default value
        // }
    }
}