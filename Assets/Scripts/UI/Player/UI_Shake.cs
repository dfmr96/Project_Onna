using System.Collections;
using UnityEngine;

public class UI_Shake : MonoBehaviour
{
    private Vector3 initialPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        initialPos = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = initialPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPos;
        shakeRoutine = null;
    }
}
