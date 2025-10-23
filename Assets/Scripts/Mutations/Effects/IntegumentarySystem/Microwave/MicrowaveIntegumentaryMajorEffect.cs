using Mutations.Core;
using Player;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Microwave Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Microwave Major")]
    public class MicrowaveIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraBurnEffect burnBehavior;

        [Header("Trigger Settings")]
        [SerializeField] private float cooldown = 2f;
        [SerializeField] private float visualDuration = 0.5f;
        [SerializeField] private float baseProcChance = 0.2f; // 20%
        [SerializeField] private float procChancePerLevel = 0.05f; // +5% por nivel

        private float lastTriggerTime;
        private PlayerModel playerModel;
        private AuraController auraCtrl;
        private AuraBurnEffect scaledBurnBehavior;
        private int currentLevel = 1;

        private void OnEnable()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Microwave Integumentary Major";
            description = "When taking damage, chance to emit thermal field that burns nearby enemies.";

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[MicrowaveMajor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[MicrowaveMajor] OnDisable called - cleaning up all references.");
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
                Debug.Log($"[MicrowaveMajor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[MicrowaveMajor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            if (!ValidateReferences())
            {
                Debug.LogError("[MicrowaveMajor] Missing required references. Check auraData and burnBehavior assignments.");
                return;
            }

            currentLevel = level;

            playerModel = player.GetComponent<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[MicrowaveMajor] PlayerModel not found!");
                return;
            }

            auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogError("[MicrowaveMajor] No AuraController found on player.");
                return;
            }

            // Clone and scale behavior
            scaledBurnBehavior = ScriptableObject.CreateInstance<AuraBurnEffect>();
            scaledBurnBehavior.burnDuration = GetScaledBurnDuration(level);
            scaledBurnBehavior.damagePerTick = burnBehavior.damagePerTick * GetValueAtLevel(level);
            scaledBurnBehavior.tickInterval = burnBehavior.tickInterval;
            scaledBurnBehavior.sourceId = "Microwave Major";

            // Subscribe to player's instance event
            playerModel.OnTakeDamage += OnPlayerDamaged;

            float procChance = GetProcChance(level);
            Debug.Log($"[MicrowaveMajor] Subscribed to player.OnTakeDamage at level {level}. Proc chance: {procChance:P0}, Burn duration: {scaledBurnBehavior.burnDuration:F1}s");

#if UNITY_EDITOR
            Debug.Log($"[MicrowaveMajor] Player has {playerModel.GetTakeDamageSubscriberCount()} active OnTakeDamage subscribers");
#endif
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[MicrowaveMajor] RemoveEffect called.");
            CleanupReferences();
        }

        private void OnPlayerDamaged(float damage)
        {
            // Validar que las referencias no sean "stale" de una sesión anterior
            if (!IsValidRuntimeState())
            {
                Debug.LogWarning("[MicrowaveMajor] OnPlayerDamaged called with stale references - ignoring.");
                return;
            }

            // Cooldown check
            if (Time.time - lastTriggerTime < cooldown)
                return;

            // Roll de probabilidad
            float roll = Random.value;
            float procChance = GetProcChance(currentLevel);

            if (roll <= procChance)
            {
                lastTriggerTime = Time.time;
                TriggerThermalField();
                Debug.Log($"[MicrowaveMajor] 🔥 THERMAL FIELD! Player took {damage} damage (roll={roll:F2} <= {procChance:F2})");
            }
            else
            {
                Debug.Log($"[MicrowaveMajor] No proc (roll={roll:F2} > {procChance:F2})");
            }
        }

        private void TriggerThermalField()
        {
            if (auraCtrl == null || scaledBurnBehavior == null || auraData == null)
            {
                Debug.LogWarning("[MicrowaveMajor] Cannot trigger thermal field - missing references.");
                return;
            }

            // Crear aura visual
            auraCtrl.AddAura(auraData, scaledBurnBehavior);

            // Aplicar un único tick de burn instantáneo
            scaledBurnBehavior.OnAuraTick(auraCtrl.transform.position, auraData.radius, LayerMask.GetMask("Enemy"));
            Debug.Log("[MicrowaveMajor] Thermal field applied burn to nearby enemies.");

            // Retirar aura visual luego de breve delay
            auraCtrl.StartCoroutine(RemoveAuraAfterDelay(visualDuration));
        }

        private System.Collections.IEnumerator RemoveAuraAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (auraCtrl != null && auraData != null)
                auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            if (!ValidateReferences())
                return "Missing configuration data.";

            float procChance = GetProcChance(level);
            float burnDuration = GetScaledBurnDuration(level);
            float dps = burnBehavior.damagePerTick * GetValueAtLevel(level);
            return $"When taking damage, {procChance:P0} chance to emit thermal field burning enemies for {dps:F1} DPS over {burnDuration:F1}s (CD {cooldown:F1}s).";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && burnBehavior != null;
        }

        private float GetProcChance(int level)
        {
            return Mathf.Clamp01(baseProcChance + (procChancePerLevel * (level - 1)));
        }

        private float GetScaledBurnDuration(int level)
        {
            return burnBehavior.burnDuration + 0.5f * (level - 1);
        }

        private bool IsValidRuntimeState()
        {
            // Verificar que todas las referencias runtime sean válidas y no "stale"
            if (playerModel == null || auraCtrl == null || scaledBurnBehavior == null)
                return false;

            // Verificar que el playerModel no esté destruido
            if (playerModel.gameObject == null)
                return false;

            return true;
        }

        private void CleanupReferences()
        {
            Debug.Log("[MicrowaveMajor] CleanupReferences - clearing all runtime state.");

            // Desuscribir eventos
            if (playerModel != null)
            {
                try
                {
                    playerModel.OnTakeDamage -= OnPlayerDamaged;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[MicrowaveMajor] Exception while unsubscribing: {e.Message}");
                }
                playerModel = null;
            }

            // Limpiar aura
            if (auraCtrl != null)
            {
                try
                {
                    if (auraData != null)
                        auraCtrl.RemoveAura(auraData.auraId);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[MicrowaveMajor] Exception while removing aura: {e.Message}");
                }
                auraCtrl = null;
            }

            // Destruir instancia runtime
            if (scaledBurnBehavior != null)
            {
                try
                {
                    Destroy(scaledBurnBehavior);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[MicrowaveMajor] Exception while destroying scaledBurnBehavior: {e.Message}");
                }
                scaledBurnBehavior = null;
            }

            // Resetear cooldown y nivel
            lastTriggerTime = 0f;
            currentLevel = 1;
        }
        #endregion
    }
}
