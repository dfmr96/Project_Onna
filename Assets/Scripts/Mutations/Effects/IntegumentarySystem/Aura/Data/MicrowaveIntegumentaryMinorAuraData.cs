using UnityEngine;

[CreateAssetMenu(fileName = "Microwave Integumentary Minor Aura", menuName = "Mutations/Aura Data/Microwave Integumentary Minor")]
public class MicrowaveIntegumentaryMinorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "MicrowaveIntegumentaryMinor";
        effectType = AuraEffectType.Burn;
        radius = 4.0f; // Campo más reducido que Major
        tickRate = 1f; // no se usa (burn instantáneo)
        auraColor = new Color(1f, 0.4f, 0f, 0.4f); // naranja menos intenso
        // visualPrefab = asignar en Inspector
    }
}
