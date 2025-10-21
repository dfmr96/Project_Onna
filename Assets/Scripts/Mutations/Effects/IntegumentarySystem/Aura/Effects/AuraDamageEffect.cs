using UnityEngine;

[CreateAssetMenu(fileName = "AuraDamageEffect", menuName = "Mutations/Aura Behaviors/Damage")]
public class AuraDamageEffect : ScriptableObject, IAuraBehavior
{
    [Header("Base Stats")]
    public float damagePerSecond = 5f;

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, mask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(damagePerSecond);
            }
        }
    }
}