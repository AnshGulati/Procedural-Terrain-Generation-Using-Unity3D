using UnityEngine;

public class ObjectDataValidator : MonoBehaviour
{
    [Header("Validation")]
    public ObjectData objectData;
    public TextureData textureData;
    public TerrainData terrainData;

    [ContextMenu("Validate ObjectData")]
    public void ValidateObjectData()
    {
        if (objectData == null)
        {
            Debug.LogError("ObjectData is not assigned!");
            return;
        }

        if (textureData == null)
        {
            Debug.LogError("TextureData is not assigned!");
            return;
        }

        if (terrainData == null)
        {
            Debug.LogError("TerrainData is not assigned!");
            return;
        }

        Debug.Log("=== ObjectData Validation ===");
        Debug.Log($"ObjectData: {objectData.name}");
        Debug.Log($"Min Object Radius: {objectData.minObjectRadius}");
        Debug.Log($"Object Seed: {objectData.objectSeed}");
        Debug.Log($"Noise Scale: {objectData.noiseScale}");
        Debug.Log($"Noise Offset: {objectData.noiseOffset}");
        Debug.Log($"Number of Placeable Objects: {objectData.placeableObjects.Length}");

        for (int i = 0; i < objectData.placeableObjects.Length; i++)
        {
            var obj = objectData.placeableObjects[i];
            Debug.Log($"Object {i}: {obj.name}");
            Debug.Log($"  - Prefab: {(obj.prefab != null ? obj.prefab.name : "NULL")}");
            Debug.Log($"  - Threshold: {obj.threshold}");
            Debug.Log($"  - Min Height: {obj.rules.minHeight}");
            Debug.Log($"  - Max Height: {obj.rules.maxHeight}");
            Debug.Log($"  - Max Slope: {obj.rules.maxSlope}");
            Debug.Log($"  - Spawnable Textures: {string.Join(", ", obj.spawnableTextureIndices)}");
        }

        Debug.Log("=== TextureData Validation ===");
        Debug.Log($"TextureData: {textureData.name}");
        Debug.Log($"Number of Layers: {textureData.layers.Length}");

        for (int i = 0; i < textureData.layers.Length; i++)
        {
            var layer = textureData.layers[i];
            Debug.Log($"Layer {i}: {(layer.texture != null ? layer.texture.name : "NULL")}");
            Debug.Log($"  - Start Height: {layer.startHeight}");
            Debug.Log($"  - Blend Strength: {layer.blendStrength}");
        }

        Debug.Log("=== TerrainData Validation ===");
        Debug.Log($"TerrainData: {terrainData.name}");
        Debug.Log($"Min Height: {terrainData.minHeight}");
        Debug.Log($"Max Height: {terrainData.maxHeight}");
        Debug.Log($"Mesh Height Multiplier: {terrainData.meshHeightMultiplier}");
        Debug.Log($"Uniform Scale: {terrainData.uniformScale}");
        Debug.Log($"Use Falloff: {terrainData.useFalloff}");
        Debug.Log($"Use Flat Shading: {terrainData.useFlatShading}");

        Debug.Log("=== Validation Complete ===");
    }

    [ContextMenu("Test Object Generation")]
    public void TestObjectGeneration()
    {
        if (objectData == null || textureData == null || terrainData == null)
        {
            Debug.LogError("Please assign all required data assets first!");
            return;
        }

        // Create a test height map
        int testSize = 100;
        float[,] testHeightMap = new float[testSize, testSize];
        
        // Fill with some test data
        for (int x = 0; x < testSize; x++)
        {
            for (int y = 0; y < testSize; y++)
            {
                testHeightMap[x, y] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
            }
        }

        Vector2 testChunkCentre = Vector2.zero;
        
        Debug.Log("=== Testing Object Generation ===");
        var spawnDataList = ObjectGenerator.Generate(objectData, textureData, testHeightMap, terrainData, testChunkCentre);
        Debug.Log($"Generated {spawnDataList.Count} objects in test");
    }
} 