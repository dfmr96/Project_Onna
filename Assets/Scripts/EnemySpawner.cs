using UnityEngine;
using System;
using UnityEngine.AI;
using Player;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject mutationCanvasPrefab;
    
    [SerializeField] private EnemySpawnInfo[] enemiesToSpawn;
    [SerializeField] private int wavesQuantity = 3;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;
    [SerializeField] private float safeDistanceFromPlayer = 5f;
    private Transform playerTransform;
    private int maxTries = 30;
    public Action OnAllWavesCompleted;
    //public Action OnWaveCompleted;    --  Se puede usar para que pase algo entre oleadas
    private int actualWave = 0;
    private int enemiesQuantity = 0;

    private void Start() 
    {
        playerTransform = PlayerHelper.GetPlayer().transform;
        StartWave();
    }
    public void StartWave()
    {
        actualWave++;
        Vector3 randomPosition;
        if (TryGetRandomNavMeshPosition(out randomPosition))
            transform.position = randomPosition;
        else Debug.LogWarning("No se encontr� posici�n v�lida sobre el NavMesh.");

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);
            enemy.GetComponent<EnemyModel>().OnDeath += OnEnemyDeath;
            enemiesQuantity++;
        }
    }
    private bool TryGetRandomNavMeshPosition(out Vector3 result)
    {
        for (int i = 0; i < maxTries; i++)
        {
            Vector3 randomPoint = spawnAreaCenter + new Vector3(UnityEngine.Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2), 0, UnityEngine.Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2));
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                if (playerTransform != null)
                {
                    if (Vector3.Distance(hit.position, playerTransform.position) >= safeDistanceFromPlayer)
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }
        }
        result = Vector3.zero;
        return false;
    }

    private GameObject GetRandomEnemyPrefab()
    {
        NormalizePercentagesToWeights();

        int totalWeight = 0;
        foreach (var enemy in enemiesToSpawn)
            totalWeight += enemy.weight;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var enemy in enemiesToSpawn)
        {
            currentSum += enemy.weight;
            if (randomValue < currentSum)
                return enemy.prefab;
        }

        return enemiesToSpawn.Length > 0 ? enemiesToSpawn[0].prefab : null;
    }

    private void NormalizePercentagesToWeights()
    {
        float totalPercent = 0f;
        foreach (var enemy in enemiesToSpawn)
            totalPercent += enemy.percentChance;

        if (totalPercent == 0f)
        {
            int equalWeight = 1;
            foreach (var enemy in enemiesToSpawn)
                enemy.weight = equalWeight;
        }
        else
        {
            for (int i = 0; i < enemiesToSpawn.Length; i++)
                enemiesToSpawn[i].weight = Mathf.RoundToInt(enemiesToSpawn[i].percentChance * 100);
        }
    }

    private void OnEnemyDeath(EnemyModel enemy)
    {
        enemy.OnDeath -= OnEnemyDeath;
        enemiesQuantity--;
        if (enemiesQuantity <= 0)
        {
            if (actualWave <= wavesQuantity)
            {
                StartWave();
                //OnWaveCompleted?.Invoke();
            }
            else
            {
                ShowMutationSelection();
                OnAllWavesCompleted?.Invoke();
            }
        }
    }
    
    private void ShowMutationSelection()
    {
        Instantiate(mutationCanvasPrefab);
    }
}
