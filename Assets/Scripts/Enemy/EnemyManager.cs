using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    //Para usarlo->
    //DeathManager.Instance.DestroyObject(_gameObject, 1f);

    public Action OnEnemyDeath;

    [SerializeField] private GameObject mutantExplotionPrefab;

    [Header("Spawners")]
    [SerializeField] public FloatingTextSpawner floatingTextSpawner;
    [SerializeField] public JumpingTextSpawner jumpingTextSpawner;
    [SerializeField] public ParticleSpawner particleSpawner;

    //Para comportamiento manada
    private List<Transform> activeEnemies = new List<Transform>();

    [SerializeField] private int maxEnemiesAround = 4;   // cantidad de posiciones disponibles alrededor del jugador
    [SerializeField] private float angleSpacing = 360f;  // se calcula automáticamente
    private Dictionary<Transform, float> enemyAngles = new Dictionary<Transform, float>();

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

    public void RegisterEnemy(Transform enemy)
    {
        //if (!activeEnemies.Contains(enemy))
        //    activeEnemies.Add(enemy);

        if (!enemyAngles.ContainsKey(enemy))
        {
            float assignedAngle = GetAvailableAngle();
            enemyAngles.Add(enemy, assignedAngle);
        }
    }

    public void UnregisterEnemy(Transform enemy)
    {
        //if (activeEnemies.Contains(enemy))
        //    activeEnemies.Remove(enemy);

        if (enemyAngles.ContainsKey(enemy))
            enemyAngles.Remove(enemy);
    }

    private float GetAvailableAngle()
    {
        if (enemyAngles.Count == 0)
            return 0f;

        angleSpacing = 360f / maxEnemiesAround;
        List<float> usedAngles = new List<float>(enemyAngles.Values);

        for (int i = 0; i < maxEnemiesAround; i++)
        {
            float candidate = i * angleSpacing;
            bool occupied = false;

            foreach (float used in usedAngles)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(candidate, used)) < angleSpacing * 0.4f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                return candidate;
        }

        // si todos están ocupados, da un ángulo random
        return UnityEngine.Random.Range(0f, 360f);
    }

    public Vector3 GetFlankPosition(Transform player, Transform enemy, float radius)
    {
        if (!enemyAngles.TryGetValue(enemy, out float angle))
            angle = 0f;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * radius;

        return player.position + offset;
    }

    //public Vector3 GetFlankPosition(Transform player, Transform requester, float radius)
    //{
    //    if (activeEnemies.Count == 0) return player.position;

    //    int index = activeEnemies.IndexOf(requester);
    //    if (index < 0) index = 0;

    //    // Repartimos a los enemigos en un círculo
    //    float angleStep = 360f / activeEnemies.Count;
    //    float angle = index * angleStep * Mathf.Deg2Rad;

    //    Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
    //    return player.position + offset;
    //}
}
