using UnityEngine;

[CreateAssetMenu(fileName = "Gamma Integumentary Major Aura", menuName = "Mutations/Aura Data/Gamma Integumentary Major")]
public class GammaIntegumentaryMajorAuraData : AuraData
{
    private void OnEnable()
    {
        auraId = "GammaIntegumentaryMajor";
        effectType = AuraEffectType.Damage;
        radius = 2.5f;
        tickRate = 0.5f;
        auraColor = new Color(0.3f, 1f, 0.4f, 0.6f); // Verde radiactivo suave
    }
}