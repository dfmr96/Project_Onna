using UnityEngine;

[CreateAssetMenu(fileName = "Alpha Integumentary Major Aura", menuName = "Mutations/Aura Data/Alpha Integumentary Major")]
public class AlphaIntegumentaryMajorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "AlphaIntegumentaryMajor";
        effectType = AuraEffectType.Damage;

        radius = 3.5f;          
        tickRate = 0.25f;       
        auraColor = new Color(1f, 0.4f, 0.1f, 0.6f);
    }
}