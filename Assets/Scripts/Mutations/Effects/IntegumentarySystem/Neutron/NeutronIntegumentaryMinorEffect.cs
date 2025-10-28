using Mutations.Core;
using Player;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Neutron Integumentary Minor", menuName = "Mutations/Effects/Integumentary System/Neutron Minor")]
    public class NeutronIntegumentaryMinorEffect : RadiationEffect
    {
        [Header("Drain Reduction Settings")]
        private float drainReduction = 0.5f; // 50% reducción (0.5x multiplier)
        private float baseDuration = 5f;
        private float durationPerLevel = 0.5f;
        private float baseProcChance = 0.2f; // 20%
        private float procChancePerLevel = 0.05f; // +5% por nivel

        private PlayerModel playerModel;
        private int currentLevel = 1;
        private Coroutine drainReductionRoutine;

        private void OnEnable()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Minor;
            effectName = "Neutron Integumentary Minor";
            description = "Upon taking damage, you gain a 20% temporary chance to reduce the loss of vital time by 50%.";

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[NeutronMinor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[NeutronMinor] OnDisable called - cleaning up all references.");
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
                Debug.Log($"[NeutronMinor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[NeutronMinor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            currentLevel = level;

            playerModel = player.GetComponent<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[NeutronMinor] PlayerModel not found!");
                return;
            }

            // Suscribirse al evento
            playerModel.OnTakeDamage += OnPlayerDamaged;

            float procChance = GetProcChance(level);
            float duration = GetDuration(level);
            Debug.Log($"[NeutronMinor] Subscribed to OnTakeDamage at level {level}. Proc chance: {procChance:P0}, Duration: {duration:F1}s");
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[NeutronMinor] RemoveEffect called.");
            CleanupReferences();
        }

        private void OnPlayerDamaged(float damage)
        {
            // Validar que las referencias no sean "stale" de una sesión anterior
            if (!IsValidRuntimeState())
            {
                Debug.LogWarning("[NeutronMinor] OnPlayerDamaged called with stale references - ignoring.");
                return;
            }

            // Roll de probabilidad
            float roll = Random.value;
            float procChance = GetProcChance(currentLevel);

            if (roll <= procChance)
            {
                // ¡Proc! Activar reducción de drain
                ActivateDrainReduction();
            }
            else
            {
                Debug.Log($"[NeutronMinor] No proc (roll={roll:F2} > {procChance:F2})");
            }
        }

        private void ActivateDrainReduction()
        {
            if (playerModel == null) return;

            // Si ya hay una rutina activa, cancelarla y reiniciar
            if (drainReductionRoutine != null)
            {
                playerModel.StopCoroutine(drainReductionRoutine);
                Debug.Log("[NeutronMinor] ⚡ Field REFRESHED! Restarting duration.");
            }
            else
            {
                Debug.Log("[NeutronMinor] ⚡ FIELD ACTIVATED! Drain reduction enabled.");
            }

            // Aplicar reducción de drain
            playerModel.SetDrainMultiplier(drainReduction);

            // Iniciar coroutine para restaurar
            drainReductionRoutine = playerModel.StartCoroutine(DrainReductionRoutine());
        }

        private IEnumerator DrainReductionRoutine()
        {
            float duration = GetDuration(currentLevel);
            yield return new WaitForSeconds(duration);

            // Restaurar drain normal
            if (playerModel != null)
            {
                playerModel.ResetDrainMultiplier();
                Debug.Log($"[NeutronMinor] ⏱️ Field expired after {duration:F1}s. Drain back to normal.");
            }

            drainReductionRoutine = null;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float procChance = GetProcChance(level);
            float duration = GetDuration(level);
            float reductionPct = (1f - drainReduction) * 100f;
            return $"When taking damage, {procChance:P0} chance to reduce vital time drain by {reductionPct:F0}% for {duration:F1}s.";
        }

        #region Helper Methods
        private float GetProcChance(int level)
        {
            return Mathf.Clamp01(baseProcChance + (procChancePerLevel * (level - 1)));
        }

        private float GetDuration(int level)
        {
            return baseDuration + (durationPerLevel * (level - 1));
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
            Debug.Log("[NeutronMinor] CleanupReferences - clearing all runtime state.");

            // Detener coroutine activa
            if (drainReductionRoutine != null && playerModel != null)
            {
                try
                {
                    playerModel.StopCoroutine(drainReductionRoutine);
                    drainReductionRoutine = null;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NeutronMinor] Exception while stopping coroutine: {e.Message}");
                }
            }

            // Restaurar drain multiplier
            if (playerModel != null)
            {
                try
                {
                    playerModel.ResetDrainMultiplier();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NeutronMinor] Exception while resetting drain: {e.Message}");
                }
            }

            // Desuscribir eventos
            if (playerModel != null)
            {
                try
                {
                    playerModel.OnTakeDamage -= OnPlayerDamaged;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NeutronMinor] Exception while unsubscribing: {e.Message}");
                }
                playerModel = null;
            }

            currentLevel = 1;
        }
        #endregion
    }
}
