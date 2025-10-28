using UnityEngine;
using TMPro;
using System;

public class JumpingCoinsText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.8f;

    private TextMeshPro textMesh;
    private Color startColor;
    private float elapsed;
    private float verticalVelocity;
    private Action onRelease;

    public void Initialize(float damageAmount, Action releaseCallback)
    {
        SetupText("+" + Mathf.RoundToInt(damageAmount).ToString() + "\u25CF", Color.yellow, releaseCallback);
    }

    public void Initialize(string text, float lifeTime, Action releaseCallback)
    {
        fadeDuration = lifeTime;
        SetupText(text, Color.yellow, releaseCallback);
    }

    private void SetupText(string content, Color color, Action releaseCallback)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        textMesh.text = content;
        startColor = color;
        elapsed = 0f;
        verticalVelocity = jumpForce;
        onRelease = releaseCallback;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        // Movimiento parabólico
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 move = Vector3.up * verticalVelocity * Time.deltaTime + Vector3.right * floatSpeed * Time.deltaTime;
        transform.position += move;

        // Mirar a la cámara
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;

        // Fade out
        float fade = Mathf.Clamp01(1 - (elapsed / fadeDuration));
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        // Cuando termina el lifetime, devolver al pool
        if (elapsed >= lifetime)
        {
            gameObject.SetActive(false);
            onRelease?.Invoke();
        }
    }
}
