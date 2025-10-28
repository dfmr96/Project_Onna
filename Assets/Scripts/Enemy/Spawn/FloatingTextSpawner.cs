using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{

    [Header("Pool Settings")]
    [SerializeField] private FloatingDamageText floatingTextPrefab;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private Transform poolParent;

    [Header("Spawn Settings")]
    [SerializeField] private float heightTextSpawn = 2f;

    private ObjectPool<FloatingDamageText> textPool;

    private void Awake()
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("[FloatingTextSpawner]  No se asignó el prefab de texto flotante en el inspector.");
            return;
        }

        textPool = new ObjectPool<FloatingDamageText>(floatingTextPrefab, initialPoolSize, poolParent);
    }



    public void SpawnFloatingText(Vector3 position, float damageValue)
    {
        if (textPool == null)
        {
            Debug.LogWarning("[FloatingTextSpawner] No floating texts available in pool.");
            return;
        }

        FloatingDamageText text = textPool.Get();

        Vector3 spawnPos = position + Vector3.up * heightTextSpawn;
        text.transform.position = spawnPos;

        text.Initialize(damageValue, () => textPool.Release(text));
    }

    public void SpawnFloatingText(Vector3 position, string customText, float lifeTime)
    {
        FloatingDamageText text = textPool.Get();
        if (text == null)
        {
            Debug.LogWarning("[FloatingTextSpawner] No floating texts available in pool.");
            return;
        }

        Vector3 spawnPos = position + Vector3.up * heightTextSpawn;
        text.transform.position = spawnPos;

        text.Initialize(customText, lifeTime, () => textPool.Release(text));
    }
}

