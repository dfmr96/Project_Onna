using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AuraRandomSlowEffect", menuName = "Mutations/Aura Behaviors/Random Slow")]
public class AuraRandomSlowEffect : ScriptableObject, IAuraBehavior
{
    [Header("Slow Settings")]
    [Range(0.1f, 1f)] public float speedMultiplier = 0.75f; // 25% slow
    [Range(0.1f, 3f)] public float duration = 1.5f;
    [Range(0f, 1f)] public float affectedFraction = 0.5f; // 50% de enemigos

    public void OnAuraTick(Vector3 origin, float radius, LayerMask mask)
    {
        var hits = Physics.OverlapSphere(origin, radius, mask);
        if (hits.Length == 0) return;

        // Mezclar aleatoriamente los enemigos
        List<Collider> candidates = new List<Collider>(hits);
        for (int i = 0; i < candidates.Count; i++)
        {
            int r = Random.Range(i, candidates.Count);
            (candidates[i], candidates[r]) = (candidates[r], candidates[i]);
        }

        // Calcular cuántos afectar
        int affectedCount = Mathf.CeilToInt(candidates.Count * affectedFraction);
        for (int i = 0; i < affectedCount; i++)
        {
            var c = candidates[i];
            if (c.TryGetComponent<ISlowable>(out var slowable))
            {
                slowable.ApplySlow(speedMultiplier, duration);
            }
        }
    }
}