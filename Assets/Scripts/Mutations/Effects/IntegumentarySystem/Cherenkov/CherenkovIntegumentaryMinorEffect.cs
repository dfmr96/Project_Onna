using Mutations.Core;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Cherenkov Integumentary Minor", menuName = "Mutations/Effects/Integumentary System/Cherenkov Minor")]
    public class CherenkovIntegumentaryMinorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraDamageReductionEffect damageReductionBehavior;

        private PlayerModel playerModel;
        private AuraController auraCtrl;
        private AuraDamageReductionEffect scaledDamageReductionBehavior;
        private int currentLevel = 1;

        private void OnEnable()
        {
            radiationType = MutationType.Cherenkov;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Minor;
            effectName = "Cherenkov Integumentary Minor";
            description = "Permanent aura that weakens nearby enemies, making them deal less damage.";

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[CherenkovMinor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[CherenkovMinor] OnDisable called - cleaning up all references.");
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
                Debug.Log($"[CherenkovMinor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[CherenkovMinor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            if (!ValidateReferences())
            {
                Debug.LogError("[CherenkovMinor] Missing required references. Check auraData and damageReductionBehavior assignments.");
                return;
            }

            currentLevel = level;

            playerModel = player.GetComponent<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[CherenkovMinor] PlayerModel not found!");
                return;
            }

            auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogError("[CherenkovMinor] No AuraController found on player.");
                return;
            }

            // Clone and scale behavior
            scaledDamageReductionBehavior = ScriptableObject.CreateInstance<AuraDamageReductionEffect>();
            scaledDamageReductionBehavior.effectDuration = damageReductionBehavior.effectDuration;
            scaledDamageReductionBehavior.outgoingDamageMultiplier = GetScaledOutgoingDamageMultiplier(level);
            scaledDamageReductionBehavior.sourceId = "Cherenkov Minor";

            // Activar aura permanente
            auraCtrl.AddAura(auraData, scaledDamageReductionBehavior);

            float reductionPercent = (1f - scaledDamageReductionBehavior.outgoingDamageMultiplier) * 100f;
            Debug.Log($"[CherenkovMinor] ⚛️ DAMAGE REDUCTION AURA ACTIVE! Level {level}, Enemy damage reduction: {reductionPercent:F0}%, Radius: {auraData.radius}");
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[CherenkovMinor] RemoveEffect called.");
            CleanupReferences();
        }

        public override string GetDescriptionAtLevel(int level)
        {
            if (!ValidateReferences())
                return "Missing configuration data.";

            float multiplier = GetScaledOutgoingDamageMultiplier(level);
            float reductionPercent = (1f - multiplier) * 100f;
            return $"Permanent aura ({auraData.radius}m) weakens enemies, making them deal -{reductionPercent:F0}% damage.";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && damageReductionBehavior != null;
        }

        private float GetScaledOutgoingDamageMultiplier(int level)
        {
            // Escala el multiplicador: 0.75 → 0.70 → 0.65 → 0.60 → 0.55
            // (enemies deal 75%→55% damage, reduction of 25%→45%)
            return damageReductionBehavior.outgoingDamageMultiplier - 0.05f * (level - 1);
        }

        private void CleanupReferences()
        {
            Debug.Log("[CherenkovMinor] CleanupReferences - clearing all runtime state.");

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
                    Debug.LogWarning($"[CherenkovMinor] Exception while removing aura: {e.Message}");
                }
                auraCtrl = null;
            }

            // Destruir instancia runtime
            if (scaledDamageReductionBehavior != null)
            {
                try
                {
                    Destroy(scaledDamageReductionBehavior);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[CherenkovMinor] Exception while destroying scaledDamageReductionBehavior: {e.Message}");
                }
                scaledDamageReductionBehavior = null;
            }

            playerModel = null;
            currentLevel = 1;
        }
        #endregion
    }
}
