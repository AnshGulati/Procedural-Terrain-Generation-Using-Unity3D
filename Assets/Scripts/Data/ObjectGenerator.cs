using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Enum to define the different terrain types for clarity (can be removed if not used elsewhere directly)
public enum TerrainType { Water, Sand, Grass, Rock, Snow }

public static class ObjectGenerator
{
    public static List<ObjectSpawnData> Generate(ObjectData objectData, TextureData textureData, float[,] heightMap, TerrainData terrainData, Vector2 chunkCentre)
    {
        // Validation checks
        if (objectData == null)
        {
            Debug.LogError("ObjectGenerator: ObjectData is null!");
            return new List<ObjectSpawnData>();
        }
        
        if (objectData.placeableObjects == null || objectData.placeableObjects.Length == 0)
        {
            Debug.LogError("ObjectGenerator: No placeable objects defined in ObjectData!");
            return new List<ObjectSpawnData>();
        }
        
        if (textureData == null)
        {
            Debug.LogError("ObjectGenerator: TextureData is null!");
            return new List<ObjectSpawnData>();
        }
        
        if (terrainData == null)
        {
            Debug.LogError("ObjectGenerator: TerrainData is null!");
            return new List<ObjectSpawnData>();
        }
        
        Debug.Log($"ObjectGenerator: ObjectData has {objectData.placeableObjects.Length} placeable objects");
        for (int i = 0; i < objectData.placeableObjects.Length; i++)
        {
            var obj = objectData.placeableObjects[i];
            Debug.Log($"Object {i}: {obj.name}, Threshold: {obj.threshold}, MinHeight: {obj.rules.minHeight}, MaxHeight: {obj.rules.maxHeight}, MaxSlope: {obj.rules.maxSlope}");
        }

        var spawnDataList = new List<ObjectSpawnData>();
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        var regionSize = new Vector2(width, height);

        Debug.Log($"ObjectGenerator: Starting generation for chunk at {chunkCentre}. Map size: {width}x{height}");

        // Use Poisson Disc Sampling to get nicely spaced points
        List<Vector2> points = PoissonDiscSampling.GeneratePoints(objectData.minObjectRadius, regionSize, objectData.objectSeed);
        
        Debug.Log($"ObjectGenerator: Generated {points.Count} candidate points");

        foreach (Vector2 point in points)
        {
            int x = Mathf.RoundToInt(point.x);
            int y = Mathf.RoundToInt(point.y);

            if (x < 0 || x >= width || y < 0 || y >= height) continue;

            // --- Start of New Logic ---

            // 1. Get all the properties of the current point
            float objectSpawnNoiseValue = Noise.GenerateNoiseMap(1, 1, objectData.objectSeed, objectData.noiseScale, 1, 1, 1, objectData.noiseOffset + new Vector2(x, y), Noise.NormalizeMode.Local)[0, 0];
            float currentTerrainNormalizedHeight = heightMap[x, y];
            float worldHeight = terrainData.meshHeightCurve.Evaluate(currentTerrainNormalizedHeight) * terrainData.meshHeightMultiplier * terrainData.uniformScale;
            float slope = GetSlope(x, y, heightMap, terrainData.meshHeightMultiplier * terrainData.uniformScale);
            int textureIndex = GetTextureIndexFromHeight(worldHeight, textureData);

            // 2. Create a list of all objects that are allowed to spawn here
            List<ObjectData.PlaceableObject> validObjects = new List<ObjectData.PlaceableObject>();
            foreach (ObjectData.PlaceableObject currentObject in objectData.placeableObjects)
            {
                bool validNoise = objectSpawnNoiseValue >= currentObject.threshold;
                bool validHeight = worldHeight >= currentObject.rules.minHeight && worldHeight <= currentObject.rules.maxHeight;
                bool validSlope = slope <= currentObject.rules.maxSlope;
                bool validTexture = currentObject.spawnableTextureIndices.Length == 0 || currentObject.spawnableTextureIndices.Contains(textureIndex);

                if (validNoise && validHeight && validSlope && validTexture)
                {
                    validObjects.Add(currentObject);
                }
            }

            // 3. If any objects were valid, pick one at random to spawn
            if (validObjects.Count > 0)
            {
                System.Random prng = new System.Random(point.GetHashCode() + objectData.objectSeed);
                ObjectData.PlaceableObject objectToSpawn = validObjects[prng.Next(0, validObjects.Count)];

                Vector3 position = new Vector3(
                    (chunkCentre.x - width / 2f + x) * terrainData.uniformScale,
                    worldHeight,
                    (chunkCentre.y - height / 2f + y) * terrainData.uniformScale
                );

                spawnDataList.Add(new ObjectSpawnData(objectToSpawn.prefab, new Vector2(position.x, position.z), Quaternion.identity, Vector3.one));
                Debug.Log($"ObjectGenerator: Valid object found - {objectToSpawn.name} at height {worldHeight}, slope {slope}, texture {textureIndex}");
            }
        }
        
        Debug.Log($"ObjectGenerator: Generated {spawnDataList.Count} objects for chunk at {chunkCentre}");
        return spawnDataList;
    }

    /// <summary>
    /// Determines the texture layer index based on the normalized terrain height.
    /// This method replaces the old GetTerrainTypeFromHeight and directly uses TextureData.
    /// </summary>
    /// <param name="normalizedHeight">The height value from the noise map (0-1 range).</param>
    /// <param name="textureData">The TextureData asset containing layer definitions.</param>
    /// <returns>The index of the texture layer.</returns>
    public static int GetTextureIndexFromHeight(float worldHeight, TextureData textureData)
    {
        // These start heights MUST match your TerrainMaterialURP settings.
        float sandStartHeight = 2.3f;
        float grassStartHeight = 11.5f;
        float rocksStartHeight = 24f;
        // We don't need a snow height here, as it's the highest layer.

        // Check from the highest layer down to the lowest
        if (worldHeight >= rocksStartHeight)
        {
            return 3; // This is the index for Rocks
        }
        if (worldHeight >= grassStartHeight)
        {
            return 2; // This is the index for Grass
        }
        if (worldHeight >= sandStartHeight)
        {
            return 1; // This is the index for Sand
        }

        return 0; // If nothing else, it must be Water
    }

    /// <summary>
    /// Calculates the slope at a given point on the height map.
    /// Uses heightMultiplier to get accurate world-space slope.
    /// </summary>
    /// <param name="x">X-coordinate on the height map.</param>
    /// <param name="z">Z-coordinate on the height map.</param>
    /// <param name="heightMap">The 2D array of normalized heights.</param>
    /// <param name="totalHeightScale">The combined height scale (meshHeightMultiplier * uniformScale).</param>
    /// <returns>The slope in degrees.</returns>
    static float GetSlope(int x, int z, float[,] heightMap, float totalHeightScale)
    {
        int width = heightMap.GetLength(0);
        int depth = heightMap.GetLength(1);

        // Get the height of the current point (scaled to world units for accurate slope calculation)
        float centerHeight = heightMap[x, z] * totalHeightScale;

        // Calculate differences in height with neighbours
        // Using ternary operators to handle edge cases where neighbours are out of bounds
        float dx = (x < width - 1 ? heightMap[x + 1, z] : centerHeight) - (x > 0 ? heightMap[x - 1, z] : centerHeight);
        float dz = (z < depth - 1 ? heightMap[x, z + 1] : centerHeight) - (z > 0 ? heightMap[x, z - 1] : centerHeight);

        // Scale differences by the total height scale
        dx *= totalHeightScale;
        dz *= totalHeightScale;

        // Calculate the magnitude of the slope vector in horizontal plane and the vertical rise
        // The '2' in Mathf.Atan2(sqrt(...), 2) is a simplification assuming a grid unit distance of 2 for slope calculation,
        // or more accurately, the horizontal distance over which dx and dz were calculated (e.g., if checking 2 units away)
        // If your horizontal units are 1, this '2' might need adjustment or be more explicit with horizontal distance.
        // For simplicity with HeightMap values, this often works out for relative slope.
        return Mathf.Atan2(Mathf.Sqrt(dx * dx + dz * dz), 2) * Mathf.Rad2Deg;
    }

    // This struct should already be in your script, but is included for completeness.
    [System.Serializable]
    public struct ObjectSpawnData
    {
        public readonly GameObject prefab;
        public readonly Vector2 position; // <-- TO A Vector2
        public readonly Quaternion rotation;
        public readonly Vector3 scale;

        public ObjectSpawnData(GameObject prefab, Vector2 position, Quaternion rotation, Vector3 scale) // <-- AND THIS
        {
            this.prefab = prefab;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}