using UnityEngine;

[CreateAssetMenu(fileName = "AuraBurnEffect", menuName = "Mutations/Aura Behaviors/Burn")]
public class AuraBurnEffect : ScriptableObject, IAuraBehavior
{
    [Header("Burn Settings")]
    public float burnDuration = 3f;
    public float damagePerTick = 2f;
    public float tickInterval = 1f;
    public string sourceId = "Microwave Major";

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, mask);
        int burnCount = 0;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IStatusAffectable>(out var target))
            {
                var burnEffect = new BurnEffect(burnDuration, damagePerTick, sourceId);
                target.ApplyStatusEffect(burnEffect);
                burnCount++;
            }
        }

        if (burnCount > 0)
        {
            Debug.Log($"[AuraBurn] Applied burn to {burnCount} enemies ({damagePerTick} DPS for {burnDuration}s)");
        }
    }
}
