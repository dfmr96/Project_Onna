using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ParticleInfo
    {
        public string id;
        public ParticleSystem prefab;
        public int initialPoolSize = 5;
    }

    [SerializeField] private List<ParticleInfo> particlesList;

    private Dictionary<string, ObjectPool<ParticleSystem>> particlePools = new Dictionary<string, ObjectPool<ParticleSystem>>();

    private void Awake()
    {
        foreach (var info in particlesList)
        {
            if (info.prefab == null) continue;
            particlePools.Add(info.id, new ObjectPool<ParticleSystem>(
                info.prefab,
                info.initialPoolSize,
                transform
            ));
        }
    }

    /// <summary>
    /// Lanza un efecto de partículas en worldPosition con rotación opcional.
    /// </summary>
    public void Spawn(string id, Vector3 position, Quaternion rotation = default, float duration = 0f)
    {
        if (!particlePools.ContainsKey(id))
        {
            Debug.LogWarning($"ParticleSpawner: No existe pool con id '{id}'");
            return;
        }

        ParticleSystem ps = particlePools[id].Get();
        ps.transform.position = position;
        ps.transform.rotation = rotation;
        ps.gameObject.SetActive(true);

        ps.Play();

        // Retornar al pool cuando termine
        StartCoroutine(ReturnToPoolWhenDone(ps, id, duration));
    }

    public void SpawnFromPrefab(ParticleSystem prefab, Vector3 position, Quaternion rotation = default, Transform parent = null, float duration = 0f)
    {
        if (prefab == null) return;

        // Crear un pool temporal si no existe
        string tempId = prefab.name + "_TempPool";
        if (!particlePools.ContainsKey(tempId))
        {
            particlePools.Add(tempId, new ObjectPool<ParticleSystem>(prefab, 2, parent != null ? parent : transform));
        }

        ParticleSystem ps = particlePools[tempId].Get();
        ps.transform.position = position;
        ps.transform.rotation = rotation;
        if (parent != null) ps.transform.SetParent(parent);
        ps.gameObject.SetActive(true);
        ps.Play();

        StartCoroutine(ReturnToPoolWhenDone(ps, tempId, duration));
    }


    private System.Collections.IEnumerator ReturnToPoolWhenDone(ParticleSystem ps, string id, float duration)
    {
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
        }
        else
        {
            yield return new WaitUntil(() => !ps.isPlaying);
        }

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
        particlePools[id].Release(ps);
    }
}
