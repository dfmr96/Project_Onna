using UnityEngine;

[CreateAssetMenu(fileName = "AuraSlowEffect", menuName = "Mutations/Aura Behaviors/Slow")]
public class AuraSlowEffect : ScriptableObject, IAuraBehavior
{
    [Range(0f, 1f)] public float slowMultiplier = 0.5f;

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, mask);
        /*foreach (var hit in hits)
        {
            var slow = hit.GetComponent<ISlowable>();
            if (slow != null)
                slow.ApplySlow(slowMultiplier);
        }*/
    }
}