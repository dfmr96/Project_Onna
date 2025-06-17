using UnityEngine;

public class EnemyShieldFx : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis = new Vector3(0, 1, 0);
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Flicker Settings")]
    [SerializeField] private Renderer shieldRenderer;
    [SerializeField] private float baseFlickerSpeed = 15f;
    [SerializeField] private float flickerIntensityMaxAlpha = 0.5f;
    [SerializeField] private float flickerIntensityMinAlpha = 0.1f;


    [Header("Random Glitch Settings")]
    [SerializeField] private float glitchChance = 0.1f;       
    [SerializeField] private float glitchDuration = 0.2f;     
    [SerializeField] private float glitchMinAlpha = 0.1f;
    [SerializeField] private float glitchMaxAlpha = 0.05f; 

    private Material shieldMat;
    private Color originalColor;
    private float glitchTimer = 0f;
    private bool isGlitching = false;

    void Start()
    {
        shieldMat = shieldRenderer.material;
        originalColor = shieldMat.color;
    }

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);

        if (!isGlitching && Random.value < glitchChance * Time.deltaTime)
        {
            isGlitching = true;
            glitchTimer = glitchDuration;
        }

        if (isGlitching)
        {
            glitchTimer -= Time.deltaTime;
            if (glitchTimer <= 0f)
            {
                isGlitching = false;
            }
        }

        float flicker = Mathf.Abs(Mathf.Sin(Time.time * baseFlickerSpeed));
        float alpha = Mathf.Lerp(flickerIntensityMinAlpha, flickerIntensityMaxAlpha, flicker);

        if (isGlitching)
        {
            alpha = Random.Range(glitchMinAlpha, glitchMaxAlpha);
        }

        Color newColor = originalColor;
        newColor.a = alpha;
        shieldMat.color = newColor;
    }
}
