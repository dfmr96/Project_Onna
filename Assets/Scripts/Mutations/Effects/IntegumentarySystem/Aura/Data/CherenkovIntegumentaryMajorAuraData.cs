using UnityEngine;

[CreateAssetMenu(fileName = "Cherenkov Integumentary Major Aura", menuName = "Mutations/Aura Data/Cherenkov Integumentary Major")]
public class CherenkovIntegumentaryMajorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "CherenkovIntegumentaryMajor";
        effectType = AuraEffectType.Weaken;
        radius = 5.0f;
        tickRate = 0.5f; // Reaplica debuff frecuentemente
        auraColor = new Color(0.3f, 0.6f, 1f, 0.5f); // Azul brillante (Cherenkov radiation)
        // visualPrefab = asignar en Inspector
    }
}