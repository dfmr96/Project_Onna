using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Mutations.Core.Categories;

namespace Mutations.Testing
{
    public class GenericMutationTestingSystem : MonoBehaviour
    {
        [Header("Mutation Effect (assign any)")]
        [SerializeField] private StatModifierEffect mutationEffect;

        [Header("Testing Settings")]
        [SerializeField] private int mutationLevel = 1;
        [SerializeField] private bool isMutationActive = false;

        [Header("Visual Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.magenta;
        [SerializeField] private float gizmoBaseRadius = 1f;

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
            if (Input.GetKeyDown(KeyCode.K)) ToggleMutation();
            if (Input.GetKeyDown(KeyCode.L)) CycleMutationLevel();
            if (Input.GetKeyDown(KeyCode.G)) guiEnabled = !guiEnabled;
        }

        private void ToggleMutation()
        {
            if (!ValidateSetup()) return;

            if (isMutationActive) RemoveMutation();
            else ApplyMutation();
        }

        private void ApplyMutation()
        {
            if (mutationEffect != null && playerGO != null)
            {
                mutationEffect.ApplyEffect(playerGO, mutationLevel);
                isMutationActive = true;

                Debug.Log($"[GenericMutationTesting] Ok {mutationEffect.name} APPLIED - Level {mutationLevel}");
                LogCurrentStats("After Apply");
            }
        }

        private void RemoveMutation()
        {
            if (mutationEffect != null && playerGO != null)
            {
                mutationEffect.RemoveEffect(playerGO);
                isMutationActive = false;

                Debug.Log($"[GenericMutationTesting] Cancel {mutationEffect.name} REMOVED");
                LogCurrentStats("After Remove");
            }
        }

        private void CycleMutationLevel()
        {
            mutationLevel = mutationLevel >= 4 ? 1 : mutationLevel + 1;
            Debug.Log($"[GenericMutationTesting]  Level changed to: {mutationLevel}");

            if (isMutationActive)
            {
                RemoveMutation();
                ApplyMutation();
            }
        }

        private void FindPlayer()
        {
            playerGO = PlayerHelper.GetPlayer();
            if (playerGO != null)
            {
                playerModel = playerGO.GetComponent<PlayerModel>();
                if (playerModel == null)
                {
                    Debug.LogError("[GenericMutationTesting] PlayerModel component not found!");
                }
            }
            else
            {
                Debug.LogError("[GenericMutationTesting] Player GameObject not found!");
            }
        }

        private bool ValidateSetup()
        {
            if (mutationEffect == null)
            {
                Debug.LogError("[GenericMutationTesting] Mutation Effect not assigned!");
                return false;
            }

            if (playerGO == null)
            {
                FindPlayer();
                if (playerGO == null)
                {
                    Debug.LogError("[GenericMutationTesting] Player not found!");
                    return false;
                }
            }

            if (playerModel?.StatContext == null)
            {
                Debug.LogError("[GenericMutationTesting] PlayerModel.StatContext is null!");
                return false;
            }

            return true;
        }

        private void LogCurrentStats(string moment)
        {
            if (playerModel?.StatContext != null)
            {
                float moveSpeed = playerModel.StatContext.Source.Get(playerModel.StatRefs.movementSpeed);
                float drainRate = playerModel.StatContext.Source.Get(playerModel.StatRefs.passiveDrainRate);

                GUILayout.Label($"Move Speed: {moveSpeed:F2}");
                GUILayout.Label($"Drain Rate: {drainRate:F2}");
            }
        }

        private void OnGUI()
        {
            if (!guiEnabled) return;

            GUI.Box(new Rect(10, 10, 300, 120), "Generic Mutation Tester");

            string statusText = isMutationActive ? $"ACTIVE (Level {mutationLevel})" : "INACTIVE";
            Color statusColor = isMutationActive ? Color.green : Color.red;

            GUI.color = statusColor;
            GUI.Label(new Rect(20, 35, 300, 20), $"Status: {statusText}");
            GUI.color = Color.white;

            GUI.Label(new Rect(20, 60, 300, 20), "Controls:");
            GUI.Label(new Rect(20, 80, 300, 20), "K - Toggle Mutation | L - Cycle Level | G - Toggle GUI");
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos || playerGO == null) return;

            if (isMutationActive)
            {
                Gizmos.color = gizmoColor;
                float radius = gizmoBaseRadius + (mutationLevel * 0.5f);
                Gizmos.DrawWireSphere(playerGO.transform.position, radius);
            }
        }
    }
}

