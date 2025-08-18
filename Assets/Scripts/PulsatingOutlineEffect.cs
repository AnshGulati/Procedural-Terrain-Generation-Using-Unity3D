using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsatingOutlineEffect : MonoBehaviour
{
    [System.Serializable]
    public class OutlineSettings
    {
        public Material material;
        public float pulseSpeed = 1.5f;
        public float minOutlineWidth = 0.00001f;
        public float maxOutlineWidth = 0.0001f;
    }

    [Header("Outline Materials with Settings")]
    [SerializeField] private List<OutlineSettings> outlines = new List<OutlineSettings>();

    private Coroutine pulseCoroutine;

    private void OnEnable()
    {
        if (outlines.Count > 0)
            pulseCoroutine = StartCoroutine(PulseOutlineCoroutine());
    }

    private IEnumerator PulseOutlineCoroutine()
    {
        while (true)
        {
            foreach (var outline in outlines)
            {
                if (outline.material != null && outline.material.HasProperty("_OutlineWidth"))
                {
                    float t = Mathf.PingPong(Time.time * outline.pulseSpeed, 1f);
                    float outlineWidth = Mathf.Lerp(outline.minOutlineWidth, outline.maxOutlineWidth, t);
                    outline.material.SetFloat("_OutlineWidth", outlineWidth);
                }
            }

            yield return null;
        }
    }

    private void OnDisable()
    {
        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        // Reset all outlines when disabled
        foreach (var outline in outlines)
        {
            if (outline.material != null && outline.material.HasProperty("_OutlineWidth"))
            {
                outline.material.SetFloat("_OutlineWidth", 0);
            }
        }
    }
}
