using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{
    public static DeathManager Instance { get; private set; }

    //Para usarlo->
    //DeathManager.Instance.DestroyObject(_gameObject, 1f);

    public Action OnEnemyDeath;

    [SerializeField] private GameObject mutantExplotionPrefab;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyObject(GameObject obj, float delay = 0f)
    {
        OnEnemyDeath?.Invoke();
        Destroy(obj, delay);
    }

    public void InstantiateMutantDeath(Transform explotionPosition, float explotionLifetime)
    {
        GameObject prefab = Instantiate(mutantExplotionPrefab, explotionPosition.position, Quaternion.identity);
        Destroy(prefab, explotionLifetime);
    }
}
