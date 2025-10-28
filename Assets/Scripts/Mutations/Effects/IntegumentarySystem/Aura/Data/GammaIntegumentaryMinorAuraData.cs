using UnityEngine;

[CreateAssetMenu(fileName = "Gamma Integumentary Minor Aura", menuName = "Mutations/Aura Data/Gamma Integumentary Minor")]
public class GammaIntegumentaryMinorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "GammaIntegumentaryMinor";
        effectType = AuraEffectType.Damage;
        radius = 2.5f;
        tickRate = 0.5f;
        auraColor = new Color(0.5f, 0.9f, 0.3f, 0.5f); // softer green
    }
}