using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScalerFx : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 targetScale = Vector3.one;

    private Coroutine currentRoutine;

    private void OnEnable()
    {
        RestartScale();
    }

    private void OnDisable()
    {
        // Cancelamos cualquier coroutine cuando se desactiva
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
    }

    public void RestartScale()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine()
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 finalScale = targetScale;
        float t = 0f;

        transform.localScale = initialScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t / duration);
            yield return null;
        }

        transform.localScale = finalScale;
        currentRoutine = null;
    }
}

