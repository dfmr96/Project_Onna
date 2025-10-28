using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float fadeDuration = 1f;

    private TextMeshPro textMesh;
    private Color startColor;
    private float elapsed;
    private System.Action onRelease;

    public void Initialize(float damageAmount, System.Action releaseCallback)
    {
        SetupText(Mathf.RoundToInt(damageAmount).ToString(), Color.white, releaseCallback);
    }

    public void Initialize(string text, float lifeTime, System.Action releaseCallback)
    {
        fadeDuration = lifeTime;
        SetupText(text, Color.yellow, releaseCallback);
    }

    private void SetupText(string content, Color color, System.Action releaseCallback)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        textMesh.text = content;
        startColor = color;
        elapsed = 0f;
        onRelease = releaseCallback;

        gameObject.SetActive(true);
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Movimiento hacia arriba
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Siempre mirar a la cámara
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;

        // Fade out
        float fade = Mathf.Clamp01(1 - (elapsed / fadeDuration));
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        // Cuando termina el lifetime devolver al pool
        if (elapsed >= lifetime)
        {
            gameObject.SetActive(false);
            onRelease?.Invoke();
        }
    }
}
