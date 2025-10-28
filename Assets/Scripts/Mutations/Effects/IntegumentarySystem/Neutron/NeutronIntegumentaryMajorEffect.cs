using Mutations.Core;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Neutron Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Neutron Major")]
    public class NeutronIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Time Recovery Settings")]
        private float timeRecovered = 10f;
        private float baseProcChance = 0.2f; // 20%
        private float procChancePerLevel = 0.05f; // +5% por nivel

        private PlayerModel playerModel;
        private int currentLevel = 1;

        private void OnEnable()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Neutron Integumentary Major";
            description = $"When taking damage, has a 20% chance to recover {timeRecovered} of vital time.";

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[NeutronMajor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[NeutronMajor] OnDisable called - cleaning up all references.");
            CleanupReferences();

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                Debug.Log($"[NeutronMajor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[NeutronMajor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            currentLevel = level;

            playerModel = player.GetComponent<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[NeutronMajor] PlayerModel not found!");
                return;
            }

            // Suscribirse al evento
            playerModel.OnTakeDamage += OnPlayerDamaged;

            float procChance = GetProcChance(level);
            Debug.Log($"[NeutronMajor] Subscribed to OnTakeDamage at level {level}. Proc chance: {procChance:P0}");
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[NeutronMajor] RemoveEffect called.");
            CleanupReferences();
        }

        private void OnPlayerDamaged(float damage)
        {
            // Validar que las referencias no sean "stale" de una sesión anterior
            if (!IsValidRuntimeState())
            {
                Debug.LogWarning("[NeutronMajor] OnPlayerDamaged called with stale references - ignoring.");
                return;
            }

            // Roll de probabilidad
            float roll = Random.value;
            float procChance = GetProcChance(currentLevel);

            if (roll <= procChance)
            {
                // ¡Proc! Recuperar tiempo vital
                if (playerModel is IHealable healable)
                {
                    healable.RecoverTime(timeRecovered);
                    Debug.Log($"[NeutronMajor] ⚡ TIME RECOVERED! Player took {damage} damage and recovered {timeRecovered}s (roll={roll:F2} <= {procChance:F2})");
                }
            }
            else
            {
                Debug.Log($"[NeutronMajor] No proc (roll={roll:F2} > {procChance:F2})");
            }
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float procChance = GetProcChance(level);
            return $"When taking damage, {procChance:P0} chance to recover +{timeRecovered:F1}s of vital time.";
        }

        #region Helper Methods
        private float GetProcChance(int level)
        {
            return Mathf.Clamp01(baseProcChance + (procChancePerLevel * (level - 1)));
        }

        private bool IsValidRuntimeState()
        {
            // Verificar que todas las referencias runtime sean válidas y no "stale"
            if (playerModel == null)
                return false;

            // Verificar que el playerModel no esté destruido
            if (playerModel.gameObject == null)
                return false;

            return true;
        }

        private void CleanupReferences()
        {
            Debug.Log("[NeutronMajor] CleanupReferences - clearing all runtime state.");

            // Desuscribir eventos
            if (playerModel != null)
            {
                try
                {
                    playerModel.OnTakeDamage -= OnPlayerDamaged;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NeutronMajor] Exception while unsubscribing: {e.Message}");
                }
                playerModel = null;
            }

            currentLevel = 1;
        }
        #endregion
    }
}
