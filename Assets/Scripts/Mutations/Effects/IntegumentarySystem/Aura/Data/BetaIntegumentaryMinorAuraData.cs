using UnityEngine;

[CreateAssetMenu(fileName = "Beta Integumentary Minor Aura", menuName = "Mutations/Aura Data/Beta Integumentary Minor")]
public class BetaIntegumentaryMinorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "BetaIntegumentaryMinor";
        effectType = AuraEffectType.Slow;
        radius = 2.8f;
        tickRate = 1f; // pulso cada segundo
        auraColor = new Color(0.5f, 0.8f, 1f, 0.35f); // azul tenue
        // visualPrefab = tu AuraVisual_Flat
    }
}