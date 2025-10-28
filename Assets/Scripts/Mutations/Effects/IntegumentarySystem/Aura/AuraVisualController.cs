using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class AuraVisualController : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseIntensity = 0.15f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool alignToGround = true;

    [Tooltip("Compensates for mesh base size (Plane=10, Quad=1, Sphere=1).")]
    [SerializeField] private float meshSizeCompensation = 10f;

    private Material auraMatInstance;
    private Color baseColor;
    private float baseScale;
    private Transform tr;

    public void Initialize(Color color, float radius)
    {
        tr = transform;
        baseColor = color;
        baseScale = radius * 2f;

        var renderer = GetComponent<MeshRenderer>();
        auraMatInstance = new Material(renderer.sharedMaterial);
        renderer.material = auraMatInstance;

        // Set emission and base map color (keep alpha from original)
        var originalBase = auraMatInstance.GetColor("_BaseColor");
        var newBase = new Color(color.r, color.g, color.b, originalBase.a);
        auraMatInstance.SetColor("_BaseColor", newBase);
        auraMatInstance.SetColor("_EmissionColor", color);

        SetRadius(radius);
    }

    public void SetRadius(float radius)
    {
        // Plane = 10 units wide by default
        baseScale = (radius * 2f) / meshSizeCompensation;
        tr.localScale = new Vector3(baseScale, baseScale, baseScale);
    }

    private void Update()
    {
        if (!auraMatInstance) return;

        // Scale pulse
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        tr.localScale = Vector3.one * (baseScale * pulse);

        // Rotate slowly
        tr.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        // Flicker only emission (keep base alpha fixed)
        Color emission = baseColor * (1f + Mathf.Sin(Time.time * pulseSpeed) * 0.25f);
        auraMatInstance.SetColor("_EmissionColor", emission);
    }

    private void OnDestroy()
    {
        if (auraMatInstance)
            Destroy(auraMatInstance);
    }
}
