using UnityEngine;

[CreateAssetMenu(fileName = "Beta Integumentary Major Aura", menuName = "Mutations/Aura Data/Beta Integumentary Major")]
public class BetaIntegumentaryMajorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "BetaIntegumentaryMajor";
        effectType = AuraEffectType.Slow;
        radius = 3.5f;
        tickRate = 0.25f;
        auraColor = new Color(0.3f, 0.7f, 1f, 0.45f); // azul frío
        // visualPrefab = AuraVisual_Flat
    }
}