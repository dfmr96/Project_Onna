using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core;   // For RadiationEffect
using Player;
using UnityEditor;

namespace Mutations.Testing
{
    public class GenericRadiationTestingSystem : MonoBehaviour
    {
        [Header("Radiation Effect (assign any)")]
        [SerializeField] private RadiationEffect radiationEffect;

        [Header("Testing Settings")]
        [SerializeField] private int mutationLevel = 1;
        [SerializeField] private bool isEffectActive = false;

        [Header("Visual Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.cyan;
        [SerializeField] private float gizmoBaseRadius = 2f;

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
            if (Input.GetKeyDown(KeyCode.K)) ToggleEffect();
            if (Input.GetKeyDown(KeyCode.L)) CycleLevel();
            if (Input.GetKeyDown(KeyCode.G)) guiEnabled = !guiEnabled;
        }

        private void ToggleEffect()
        {
            if (!ValidateSetup()) return;

            if (isEffectActive) RemoveEffect();
            else ApplyEffect();
        }

        private void ApplyEffect()
        {
            radiationEffect.ApplyEffect(playerGO, mutationLevel);
            isEffectActive = true;

            Debug.Log($"[RadiationTesting] ✅ {radiationEffect.name} APPLIED - Level {mutationLevel}");
            
        }

        private void RemoveEffect()
        {
            radiationEffect.RemoveEffect(playerGO);
            isEffectActive = false;

            Debug.Log($"[RadiationTesting] ❌ {radiationEffect.name} REMOVED");
        }

        private void CycleLevel()
        {
            mutationLevel = mutationLevel >= 4 ? 1 : mutationLevel + 1;
            Debug.Log($"[RadiationTesting] Level changed to {mutationLevel}");

            if (isEffectActive)
            {
                RemoveEffect();
                ApplyEffect();
            }
        }

        private void FindPlayer()
        {
            playerGO = PlayerHelper.GetPlayer();
            if (playerGO == null)
                Debug.LogError("[RadiationTesting] Player GameObject not found!");
        }

        private bool ValidateSetup()
        {
            if (radiationEffect == null)
            {
                Debug.LogError("[RadiationTesting] No RadiationEffect assigned!");
                return false;
            }

            if (playerGO == null)
            {
                FindPlayer();
                if (playerGO == null)
                {
                    Debug.LogError("[RadiationTesting] Player not found!");
                    return false;
                }
            }

            return true;
        }

        private void OnGUI()
        {
            if (!guiEnabled) return;

            GUI.Box(new Rect(10, 10, 300, 120), "Radiation Effect Tester");

            string statusText = isEffectActive ? $"ACTIVE (Level {mutationLevel})" : "INACTIVE";
            Color statusColor = isEffectActive ? Color.green : Color.red;

            GUI.color = statusColor;
            GUI.Label(new Rect(20, 35, 300, 20), $"Status: {statusText}");
            GUI.color = Color.white;

            GUI.Label(new Rect(20, 60, 300, 20), "Controls:");
            GUI.Label(new Rect(20, 80, 300, 20), "K - Toggle Effect | L - Cycle Level | G - Toggle GUI");
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos || playerGO == null) return;

            if (isEffectActive)
            {
                Gizmos.color = gizmoColor;
                float radius = gizmoBaseRadius + (mutationLevel * 0.5f);
                Gizmos.DrawWireSphere(playerGO.transform.position, radius);
            }
        }
    }
    
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class PlayModeEventCleaner
    {
        static PlayModeEventCleaner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Resources.UnloadUnusedAssets(); // limpia ScriptableObjects en memoria
            }
        }
    }
#endif
}
