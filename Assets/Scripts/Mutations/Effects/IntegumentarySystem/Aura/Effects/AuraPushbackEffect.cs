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
        foreach (var hit in hits)
        {
            var rb = hit.attachedRigidbody;
            if (rb == null) continue;

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
        }
    }
}