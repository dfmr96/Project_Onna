using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Collections;
using UnityEngine;
[DefaultExecutionOrder(100)]
public class AuraController : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField] private LayerMask targetMask;

    [Header("Runtime Debug (Read-Only)")]
    [SerializeField, ReadOnly] 
    private SerializedDictionary<string, AuraRuntime> activeAuras = new();

    private void Update()
    {
        foreach (var pair in activeAuras)
        {
            AuraRuntime aura = pair.Value;
            aura.timer += Time.deltaTime;

            if (aura.timer >= aura.data.tickRate)
            {
                aura.timer = 0f;
                aura.behavior?.OnAuraTick(transform.position, aura.data.radius, targetMask);
            }

            // Optional: update visual pulse or scale if needed
        }
    }

    public void AddAura(AuraData data, IAuraBehavior behavior)
    {
        if (activeAuras.ContainsKey(data.auraId))
            return;

        var runtime = new AuraRuntime(data, behavior);

        // Instantiate visual if available
        if (data.visualPrefab)
        {
            runtime.visual = Instantiate(data.visualPrefab, transform);
            runtime.visual.transform.localScale = Vector3.one * data.radius;

            var visualCtrl = runtime.visual.GetComponent<AuraVisualController>();
            visualCtrl?.Initialize(data.auraColor, data.radius);
        }

        activeAuras.Add(data.auraId, runtime);
    }

    public void RemoveAura(string id)
    {
        if (!activeAuras.TryGetValue(id, out var aura))
            return;

        if (aura.visual)
            Destroy(aura.visual);

        activeAuras.Remove(id);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (activeAuras == null || activeAuras.Count == 0) return;

        foreach (var kvp in activeAuras)
        {
            AuraRuntime aura = kvp.Value;
            if (aura == null || aura.data == null) continue;

            Gizmos.color = aura.data.auraColor;
            Gizmos.DrawWireSphere(transform.position, aura.data.radius);
        }
    }


    [System.Serializable]
    public class AuraRuntime
    {
        public AuraData data;
        public GameObject visual;
        public float timer;

        [System.NonSerialized] public IAuraBehavior behavior;

        public AuraRuntime(AuraData data, IAuraBehavior behavior)
        {
            this.data = data;
            this.behavior = behavior;
        }
    }

}
