using UnityEngine;

[CreateAssetMenu(fileName = "AuraPushbackEffect", menuName = "Mutations/Aura Behaviors/Pushback")]
public class AuraPushbackEffect : ScriptableObject, IAuraBehavior
{
    [Header("Pushback")]
    public float pushForce = 12f;
    public float upModifier = 0.25f;
    public bool useExplosion = true;

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        var hits = Physics.OverlapSphere(origin, radius, mask);
        Debug.Log($"[AuraPushback] OverlapSphere found {hits.Length} colliders");

        foreach (var hit in hits)
        {
            // Intento 1: Empujar usando Rigidbody (física real)
            var rb = hit.attachedRigidbody;
            if (rb != null)
            {
                if (useExplosion)
                {
                    rb.AddExplosionForce(pushForce, origin, radius, upModifier, ForceMode.Impulse);
                }
                else
                {
                    Vector3 dir = (rb.worldCenterOfMass - origin).normalized;
                    dir.y += upModifier;
                    rb.AddForce(dir * pushForce, ForceMode.Impulse);
                }
                Debug.Log($"[AuraPushback] Applied physics push to {hit.name}");
                continue;
            }

            // Intento 2: Empujar usando IPushable (movimiento manual)
            var pushable = hit.GetComponent<IPushable>();
            if (pushable != null)
            {
                Vector3 dir = (hit.transform.position - origin).normalized;
                dir.y += upModifier;
                dir.Normalize();
                pushable.ApplyPushback(dir, pushForce);
                Debug.Log($"[AuraPushback] Applied IPushable push to {hit.name}");
                continue;
            }

            Debug.LogWarning($"[AuraPushback] {hit.name} has no Rigidbody or IPushable - cannot push!");
        }
    }
}