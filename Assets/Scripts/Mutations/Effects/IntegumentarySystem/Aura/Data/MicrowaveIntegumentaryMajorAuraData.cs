using UnityEngine;

[CreateAssetMenu(fileName = "Microwave Integumentary Major Aura", menuName = "Mutations/Aura Data/Microwave Integumentary Major")]
public class MicrowaveIntegumentaryMajorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "MicrowaveIntegumentaryMajor";
        effectType = AuraEffectType.Burn;
        radius = 6.0f;
        tickRate = 1f; // no se usa (burn instantáneo)
        auraColor = new Color(1f, 0.3f, 0f, 0.5f); // naranja/rojo intenso
        // visualPrefab = asignar en Inspector
    }
}
