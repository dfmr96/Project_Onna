using UnityEngine;
using UnityEngine.UI;

public class UIEyeFollow : MonoBehaviour
{
    [System.Serializable]
    public class EyeLayer
    {
        public RectTransform rect;
        [Range(0f, 1f)] public float followStrength = 0.5f;
    }

    [Header("Capas del ojo (fondo → frente)")]
    public EyeLayer[] layers;

    [Header("Movimiento máximo (en píxeles)")]
    public float maxOffset = 20f;

    private RectTransform centerRect;
    private Canvas canvas;

    private void Start()
    {
        centerRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("Este script necesita estar dentro de un Canvas.");
    }

    private void Update()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            centerRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out mousePos
        );

        mousePos = Vector2.ClampMagnitude(mousePos, maxOffset);

        foreach (var layer in layers)
        {
            if (layer.rect == null) continue;

            Vector2 target = mousePos * layer.followStrength;
            layer.rect.anchoredPosition = Vector2.Lerp(layer.rect.anchoredPosition, target, Time.deltaTime * 10f);
        }
    }
}
