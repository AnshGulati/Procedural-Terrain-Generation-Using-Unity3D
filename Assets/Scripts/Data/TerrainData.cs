using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    public float uniformScale = 2.5f;

    public bool useFlatShading;
    public bool useFalloff;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public float minHeight
    {
        get
        {
            return meshHeightMultiplier*uniformScale*meshHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return meshHeightMultiplier * uniformScale * meshHeightCurve.Evaluate(1);
        }
    }

}
