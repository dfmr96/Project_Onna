using UnityEngine;

[CreateAssetMenu(fileName = "AuraSlowEffect", menuName = "Mutations/Aura Behaviors/Slow")]
public class AuraSlowEffect : ScriptableObject, IAuraBehavior
{
    [Header("Slow Settings")]
    [Range(0.1f, 1f)] public float speedMultiplier = 0.7f; // 0.7 = 30% slow
    [Range(0.1f, 2f)] public float duration = 1.0f;
    public string sourceId = "Beta Integumentary";

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        var hits = Physics.OverlapSphere(origin, radius, mask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IStatusAffectable>(out var target))
            {
                var slowEffect = new SlowEffect(duration, speedMultiplier, sourceId);
                target.ApplyStatusEffect(slowEffect);
            }
        }
    }
}