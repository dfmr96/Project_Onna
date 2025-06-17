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

    public void Initialize(float damageAmount)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = Mathf.RoundToInt(damageAmount).ToString();
        startColor = textMesh.color;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        //Movimiento hacia arriba
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        //Siempre mirar a la camara
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        //Fade out
        float fade = Mathf.Clamp01(1 - (elapsed / fadeDuration));
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

