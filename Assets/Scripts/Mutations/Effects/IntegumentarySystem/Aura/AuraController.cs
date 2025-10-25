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

            // Draw circle with aura color
            Gizmos.color = new Color(aura.data.auraColor.r, aura.data.auraColor.g, aura.data.auraColor.b, 0.5f);
            DrawCircle(transform.position, aura.data.radius, 64);

            // Draw filled circle with transparency
            Color fillColor = new Color(aura.data.auraColor.r, aura.data.auraColor.g, aura.data.auraColor.b, 0.15f);
            Gizmos.color = fillColor;
            DrawFilledCircle(transform.position, aura.data.radius, 32);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a simple gizmo even when not selected to show the controller is present
        if (activeAuras != null && activeAuras.Count > 0)
        {
            foreach (var kvp in activeAuras)
            {
                AuraRuntime aura = kvp.Value;
                if (aura == null || aura.data == null) continue;

                // Very subtle visualization when not selected
                Gizmos.color = new Color(aura.data.auraColor.r, aura.data.auraColor.g, aura.data.auraColor.b, 0.2f);
                DrawCircle(transform.position, aura.data.radius, 32);
            }
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    private void DrawFilledCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, 0, Mathf.Sin(angle1) * radius);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * radius, 0, Mathf.Sin(angle2) * radius);

            // Draw triangle from center to edge
            Gizmos.DrawLine(center, point1);
            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, center);
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
