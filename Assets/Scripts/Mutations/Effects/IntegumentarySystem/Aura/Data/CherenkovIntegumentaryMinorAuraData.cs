using UnityEngine;

[CreateAssetMenu(fileName = "Cherenkov Integumentary Minor Aura", menuName = "Mutations/Aura Data/Cherenkov Integumentary Minor")]
public class CherenkovIntegumentaryMinorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "CherenkovIntegumentaryMinor";
        effectType = AuraEffectType.Weaken;
        radius = 4.0f; // Smaller radius than Major
        tickRate = 0.5f; // Reapply debuff frequently
        auraColor = new Color(0.4f, 0.7f, 1f, 0.4f); // Lighter blue (Cherenkov radiation)
        // visualPrefab = assign in Inspector
    }
}
