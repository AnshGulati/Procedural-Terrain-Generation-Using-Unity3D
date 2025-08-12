//using UnityEngine;
//using System.Collections;

//[CreateAssetMenu()]
//public class NoiseData : UpdatableData
//{

//    public Noise.NormalizeMode normalizeMode;

//    public float noiseScale;

//    public int octaves;
//    [Range(0, 1)]
//    public float persistance;
//    public float lacunarity;

//    public int seed;
//    public Vector2 offset;


//    protected override void OnValidate()
//    {
//        if (lacunarity < 1)
//        {
//            lacunarity = 1;
//        }
//        if (octaves < 0)
//        {
//            octaves = 0;
//        }

//        base.OnValidate();
//    }

//}

using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{

    public Noise.NormalizeMode normalizeMode;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    // The 'override' keyword and the 'base.OnValidate()' call have been removed.
    protected void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}