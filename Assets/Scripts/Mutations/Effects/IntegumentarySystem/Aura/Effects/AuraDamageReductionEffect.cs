using UnityEngine;

[CreateAssetMenu(fileName = "AuraDamageReductionEffect", menuName = "Mutations/Aura Behaviors/Damage Reduction")]
public class AuraDamageReductionEffect : ScriptableObject, IAuraBehavior
{
    [Header("Damage Reduction Settings")]
    public float effectDuration = 2f;
    public float outgoingDamageMultiplier = 0.75f; // 0.75 = enemies deal 75% damage (25% reduction)
    public string sourceId = "Cherenkov Minor";

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, mask);
        int affectedCount = 0;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IStatusAffectable>(out var target))
            {
                var damageReductionEffect = new DamageReductionEffect(effectDuration, outgoingDamageMultiplier, sourceId);
                target.ApplyStatusEffect(damageReductionEffect);
                affectedCount++;
            }
        }

        if (affectedCount > 0)
        {
            float reductionPercent = (1f - outgoingDamageMultiplier) * 100f;
            Debug.Log($"[AuraDamageReduction] Weakened {affectedCount} enemies ({reductionPercent:F0}% damage reduction for {effectDuration}s)");
        }
    }
}
