using UnityEngine;
using Mutations.Effects.NervousSystem;
using Enemy.Spawn;
using Player;

namespace Mutations.Testing
{
    public class GammaNervousMinorTestingSystem : MonoBehaviour
    {
        [Header("Mutation Effect")]
        [SerializeField] private GammaNervousMinorEffect gammaNervousMinorEffect;

        [Header("Testing Settings")]
        [SerializeField] private int mutationLevel = 1;
        [SerializeField] private bool isMutationActive = false;

        [Header("Orb Spawning")]
        [SerializeField] private RastroOrb orbPrefab;
        [SerializeField] private int orbsToSpawn = 5;
        [SerializeField] private float spawnRadius = 15f;
        [SerializeField] private float spawnHeight = 1f;

        [Header("Visual Debug")]
        [SerializeField] private bool showAttractionRange = true;
        [SerializeField] private Color attractionRangeColor = Color.cyan;

        private PlayerModel playerModel;
        private GameObject playerGO;
        private bool guiEnabled = true;

        private void Start()
        {
            FindPlayer();
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // K = Toggle mutation (Level 1-4)
            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleMutation();
            }

            // L = Cycle mutation level (1-4)
            if (Input.GetKeyDown(KeyCode.L))
            {
                CycleMutationLevel();
            }

            // O = Spawn orbs around player
            if (Input.GetKeyDown(KeyCode.O))
            {
                SpawnTestOrbs();
            }

            // C = Clear all orbs
            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearAllOrbs();
            }

            // G = Toggle GUI
            if (Input.GetKeyDown(KeyCode.G))
            {
                guiEnabled = !guiEnabled;
            }
        }

        private void ToggleMutation()
        {
            if (!ValidateSetup()) return;

            if (isMutationActive)
            {
                RemoveMutation();
            }
            else
            {
                ApplyMutation();
            }
        }

        private void ApplyMutation()
        {
            if (gammaNervousMinorEffect != null && playerGO != null)
            {
                gammaNervousMinorEffect.ApplyEffect(playerGO, mutationLevel);
                isMutationActive = true;

                float range = gammaNervousMinorEffect.GetAttractRangeAtLevel(mutationLevel);
                float speed = gammaNervousMinorEffect.GetAttractSpeedAtLevel(mutationLevel);

                Debug.Log($"[GammaNervousMinorTesting] ✅ Mutation APPLIED - Level {mutationLevel}: Range {range:F1}m, Speed x{speed:F1}");
                LogCurrentStats("After Apply");
            }
        }

        private void RemoveMutation()
        {
            if (gammaNervousMinorEffect != null && playerGO != null)
            {
                gammaNervousMinorEffect.RemoveEffect(playerGO);
                isMutationActive = false;

                Debug.Log($"[GammaNervousMinorTesting] ❌ Mutation REMOVED");
                LogCurrentStats("After Remove");
            }
        }

        private void CycleMutationLevel()
        {
            mutationLevel = mutationLevel >= 4 ? 1 : mutationLevel + 1;
            Debug.Log($"[GammaNervousMinorTesting] 🔄 Level changed to: {mutationLevel}");

            // Si la mutación está activa, re-aplicarla con el nuevo level
            if (isMutationActive)
            {
                RemoveMutation();
                ApplyMutation();
            }
        }

        private void SpawnTestOrbs()
        {
            if (orbPrefab == null || playerGO == null)
            {
                Debug.LogWarning("[GammaNervousMinorTesting] ⚠️ Missing orb prefab or player!");
                return;
            }

            Vector3 playerPos = playerGO.transform.position;

            for (int i = 0; i < orbsToSpawn; i++)
            {
                // Spawn orbs en círculo alrededor del player
                float angle = (360f / orbsToSpawn) * i * Mathf.Deg2Rad;
                Vector3 spawnPos = playerPos + new Vector3(
                    Mathf.Cos(angle) * spawnRadius,
                    spawnHeight,
                    Mathf.Sin(angle) * spawnRadius
                );

                RastroOrb newOrb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

                // Configurar el orb con stats del player si la mutación está activa
                if (isMutationActive && playerModel != null)
                {
                    UpdateOrbWithPlayerStats(newOrb);
                }
            }

            Debug.Log($"[GammaNervousMinorTesting] 🌟 Spawned {orbsToSpawn} orbs around player");
        }

        private void UpdateOrbWithPlayerStats(RastroOrb orb)
        {
            // ✅ El RastroOrb ya está integrado con los stats del player automáticamente
            // Los orbs usan GetCurrentAttractionRadius() y GetCurrentAttractionSpeed()
            // que leen dinámicamente los stats del player en tiempo real

            if (playerModel?.StatContext?.Source != null)
            {
                float playerAttractRange = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractRange);
                float playerAttractSpeed = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractSpeed);

                Debug.Log($"[GammaNervousMinorTesting] 📊 Player Stats Applied to Orb - Range: +{playerAttractRange:F1}m, Speed: +{playerAttractSpeed:F1}x");
            }
        }

        private void ClearAllOrbs()
        {
            RastroOrb[] allOrbs = FindObjectsOfType<RastroOrb>();
            for (int i = allOrbs.Length - 1; i >= 0; i--)
            {
                if (allOrbs[i] != null)
                {
                    DestroyImmediate(allOrbs[i].gameObject);
                }
            }

            Debug.Log($"[GammaNervousMinorTesting] 🧹 Cleared {allOrbs.Length} orbs from scene");
        }

        private void FindPlayer()
        {
            playerGO = PlayerHelper.GetPlayer();
            if (playerGO != null)
            {
                playerModel = playerGO.GetComponent<PlayerModel>();
                if (playerModel == null)
                {
                    Debug.LogError("[GammaNervousMinorTesting] PlayerModel component not found!");
                }
            }
            else
            {
                Debug.LogError("[GammaNervousMinorTesting] Player GameObject not found!");
            }
        }

        private bool ValidateSetup()
        {
            if (gammaNervousMinorEffect == null)
            {
                Debug.LogError("[GammaNervousMinorTesting] Gamma Nervous Minor Effect not assigned!");
                return false;
            }

            if (playerGO == null)
            {
                FindPlayer();
                if (playerGO == null)
                {
                    Debug.LogError("[GammaNervousMinorTesting] Player not found!");
                    return false;
                }
            }

            if (playerModel?.StatContext == null)
            {
                Debug.LogError("[GammaNervousMinorTesting] PlayerModel.StatContext is null!");
                return false;
            }

            return true;
        }

        private void LogCurrentStats(string moment)
        {
            if (playerModel?.StatContext?.Source == null || gammaNervousMinorEffect == null) return;

            float currentRange = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractRange);
            float currentSpeed = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractSpeed);
            float expectedRange = gammaNervousMinorEffect.GetAttractRangeAtLevel(mutationLevel);
            float expectedSpeed = gammaNervousMinorEffect.GetAttractSpeedAtLevel(mutationLevel);

            Debug.Log($"[GammaNervousMinorTesting] 📊 {moment} Stats:");
            Debug.Log($"  Current Range: {currentRange:F1}m | Expected: {expectedRange:F1}m");
            Debug.Log($"  Current Speed: {currentSpeed:F1}x | Expected: {expectedSpeed:F1}x");
        }

        private void OnGUI()
        {
            if (!guiEnabled) return;

            // Panel principal
            GUI.Box(new Rect(10, 10, 320, 200), "Gamma Nervous Minor Tester");

            // Estado actual
            string statusText = isMutationActive ? $"ACTIVE (Level {mutationLevel})" : "INACTIVE";
            Color statusColor = isMutationActive ? Color.green : Color.red;

            GUI.color = statusColor;
            GUI.Label(new Rect(20, 35, 300, 20), $"Status: {statusText}");
            GUI.color = Color.white;

            // Stats actuales si la mutación está activa
            if (isMutationActive && playerModel?.StatContext?.Source != null)
            {
                float currentRange = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractRange);
                float currentSpeed = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractSpeed);

                GUI.Label(new Rect(20, 55, 300, 20), $"Current Range: {currentRange:F1}m");
                GUI.Label(new Rect(20, 75, 300, 20), $"Current Speed: {currentSpeed:F1}x");
            }

            // Controles
            GUI.Label(new Rect(20, 100, 300, 20), "Controls:");
            GUI.Label(new Rect(20, 120, 300, 20), "K - Toggle Mutation");
            GUI.Label(new Rect(20, 135, 300, 20), "L - Cycle Level (1-4)");
            GUI.Label(new Rect(20, 150, 300, 20), "O - Spawn Test Orbs");
            GUI.Label(new Rect(20, 165, 300, 20), "C - Clear All Orbs");
            GUI.Label(new Rect(20, 180, 300, 20), "G - Toggle This GUI");
        }

        private void OnDrawGizmos()
        {
            if (!showAttractionRange || playerGO == null) return;

            // Mostrar el rango de atracción actual
            if (isMutationActive && playerModel?.StatContext?.Source != null)
            {
                float currentRange = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractRange);

                Gizmos.color = attractionRangeColor;
                Gizmos.DrawWireSphere(playerGO.transform.position, currentRange);

                // Mostrar también el rango base para comparación
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(playerGO.transform.position, 5f); // Rango base típico
            }
        }
    }
}