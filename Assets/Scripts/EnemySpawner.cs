using UnityEngine;
using System;
using UnityEngine.AI;
using Player;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Orb Mutation Settings")]
    [SerializeField] private GameObject orbMutationPrefab;
    [SerializeField] private float orbMutationYOffset = 2f;

    [Header("Enemy Spawning Settings")]
    [SerializeField] private float initialDelaySpawn = 3f;
    [SerializeField] private float particleLifeTime = 4f;
    [SerializeField] private GameObject spawnParticlesPrefab;
    //[SerializeField] private GameObject mutationCanvasPrefab;
    [SerializeField] private EnemySpawnInfo[] enemiesToSpawn;
    [SerializeField] private int wavesQuantity = 3;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Vector3 spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize;
    [SerializeField] private float safeDistanceFromPlayer = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip orbSpawnSound;
    [SerializeField] private AudioSource audioSource;






    private Transform playerTransform;
    private int maxTries = 30;
    public Action OnAllWavesCompleted;
    private int actualWave = 0;
    private int enemiesQuantity = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
            if (audioSource == null)
                Debug.LogWarning("EnemySpawner: No se encontró AudioSource para reproducir orbSpawnSound");
        }
    }


    private void Start() 
    {
        if (PlayerHelper.GetPlayer() == null) return;
        playerTransform = PlayerHelper.GetPlayer().transform;
        StartWave();
    }



    public void StartWave()
    {
        actualWave++;
        Vector3 randomPosition;
        if (TryGetRandomNavMeshPosition(out randomPosition))
            transform.position = randomPosition;
        else Debug.LogWarning("No se encontro posicion valida sobre el NavMesh");


        if (spawnParticlesPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            GameObject particles = Instantiate(spawnParticlesPrefab, spawnPos, Quaternion.Euler(0f, 0f, 0f));

            Destroy(particles, particleLifeTime);
        }

        StartCoroutine(DelayedStartWave());

    }

    private IEnumerator DelayedStartWave()
    {
        yield return new WaitForSeconds(initialDelaySpawn);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            GameObject enemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);

            //Activar el efecto de spawn visual
            EnemyView enemyView = enemy.GetComponent<EnemyView>();
            if (enemyView != null)
                enemyView.PlaySpawnEffect();

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
            }
            else
            {
                SpawnOrbMutation(enemy.transform.position, orbMutationPrefab);

                // 🔹 Reproducir sonido
                if (audioSource != null && orbSpawnSound != null)
                    audioSource.PlayOneShot(orbSpawnSound);

                OnAllWavesCompleted?.Invoke();
            }
        }
    }


    //[ContextMenu("Show mutation selection")]
    //private void ShowMutationSelection() => Instantiate(mutationCanvasPrefab);

    private void SpawnOrbMutation(Vector3 position, GameObject prefab)
    {
        Instantiate(
                    prefab,
                    position + Vector3.up * orbMutationYOffset,
                    Quaternion.identity
                );
    }
}
