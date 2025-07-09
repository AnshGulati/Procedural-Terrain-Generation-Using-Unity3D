using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    public Color[] baseColours;
    public float[] baseStartHeights;

    float savedMinHeight;
    float savedMaxHeight;
    public void ApplyToMaterial(Material material)
    {
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material,float minHeight,float maxHeight)
    {
        savedMaxHeight = maxHeight;
        savedMinHeight = minHeight;

        material.SetFloat("minHeight",minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}