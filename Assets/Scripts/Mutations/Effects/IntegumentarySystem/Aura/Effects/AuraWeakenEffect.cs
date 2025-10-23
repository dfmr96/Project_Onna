using UnityEngine;

[CreateAssetMenu(fileName = "AuraWeakenEffect", menuName = "Mutations/Aura Behaviors/Weaken")]
public class AuraWeakenEffect : ScriptableObject, IAuraBehavior
{
    [Header("Weaken Settings")]
    public float weakenDuration = 2f;
    public float damageMultiplier = 1.25f; // 1.25 = enemies take 25% more damage
    public string sourceId = "Cherenkov Major";

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, mask);
        int weakenCount = 0;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IStatusAffectable>(out var target))
            {
                var weakenEffect = new WeakenEffect(weakenDuration, damageMultiplier, sourceId);
                target.ApplyStatusEffect(weakenEffect);
                weakenCount++;
            }
        }

        if (weakenCount > 0)
        {
            Debug.Log($"[AuraWeaken] Weakened {weakenCount} enemies ({damageMultiplier:P0} damage multiplier for {weakenDuration}s)");
        }
    }
}