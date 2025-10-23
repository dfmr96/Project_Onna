using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingTextSpawner : MonoBehaviour
{

    [Header("Pool Settings")]
    [SerializeField] private JumpingCoinsText jumpingTextPrefab;
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private Transform poolParent;

    [Header("Spawn Settings")]
    [SerializeField] private float heightTextSpawn = 2f;

    private ObjectPool<JumpingCoinsText> textPool;

    private void Awake()
    {
        if (jumpingTextPrefab == null)
        {
            Debug.LogError("[FloatingTextSpawner]  No se asignó el prefab de texto flotante en el inspector.");
            return;
        }

        textPool = new ObjectPool<JumpingCoinsText>(jumpingTextPrefab, initialPoolSize, poolParent);
    }



    public void SpawnFloatingText(Vector3 position, float damageValue)
    {
        if (textPool == null)
        {
            Debug.LogWarning("[FloatingTextSpawner] No floating texts available in pool.");
            return;
        }

        JumpingCoinsText text = textPool.Get();

        Vector3 spawnPos = position + Vector3.up * heightTextSpawn;
        text.transform.position = spawnPos;

        text.Initialize(damageValue, () => textPool.Release(text));
    }

    public void SpawnFloatingText(Vector3 position, string customText, float lifeTime)
    {
        JumpingCoinsText text = textPool.Get();
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


