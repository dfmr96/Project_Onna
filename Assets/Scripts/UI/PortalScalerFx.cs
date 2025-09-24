using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PortalScalerFx : MonoBehaviour
{
    [SerializeField] private bool applyScaler = true;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float startScale = 1f;
    [SerializeField] private float endScale = 0.5f;

    private Vector3 startScaleXZ;
    private Vector3 endScaleXZ;
    void Start()
    {
        if (applyScaler) StartCoroutine(ScaleXZ());
    }

    IEnumerator ScaleXZ()
    {
        startScaleXZ = new Vector3(startScale, transform.localScale.y, startScale);
        endScaleXZ = new Vector3(endScale, transform.localScale.y, endScale);
        float elapsed = 0f;

        transform.localScale = startScaleXZ;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScaleXZ, endScaleXZ, t);
            yield return null;
        }

        transform.localScale = endScaleXZ;
    }
}

