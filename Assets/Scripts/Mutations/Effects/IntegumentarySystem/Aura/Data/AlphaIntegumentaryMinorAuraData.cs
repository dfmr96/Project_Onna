using UnityEngine;

[CreateAssetMenu(fileName = "Alpha Integumentary Minor Aura", menuName = "Mutations/Aura Data/Alpha Integumentary Minor")]
public class AlphaIntegumentaryMinorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "AlphaIntegumentaryMinor";
        effectType = AuraEffectType.Pushback;  // <- clave
        radius = 3.0f;
        tickRate = 1f; // no se usa (pulso instantáneo)
        auraColor = new Color(1f, 0.85f, 0.4f, 0.4f); // dorado suave
        // visualPrefab = tu AuraVisual_Flat
    }
}