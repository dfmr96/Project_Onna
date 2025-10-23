using Mutations.Core;
using Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Cherenkov Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Cherenkov Major")]
    public class CherenkovIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraWeakenEffect weakenBehavior;

        private PlayerModel playerModel;
        private AuraController auraCtrl;
        private AuraWeakenEffect scaledWeakenBehavior;
        private int currentLevel = 1;

        private void OnEnable()
        {
            radiationType = MutationType.Cherenkov;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Cherenkov Integumentary Major";
            description = "Permanent aura that weakens nearby enemies, making them take more damage.";

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log("[CherenkovMajor] OnEnable called - subscribed to playModeStateChanged.");
#endif
        }

        private void OnDisable()
        {
            Debug.Log("[CherenkovMajor] OnDisable called - cleaning up all references.");
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
                Debug.Log($"[CherenkovMajor] Play mode state changed to {state} - forcing cleanup.");
                CleanupReferences();
            }
        }
#endif

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // Cleanup preventivo: limpiar cualquier estado de sesión anterior
            Debug.Log("[CherenkovMajor] ApplyEffect - performing preventive cleanup.");
            CleanupReferences();

            if (!ValidateReferences())
            {
                Debug.LogError("[CherenkovMajor] Missing required references. Check auraData and weakenBehavior assignments.");
                return;
            }

            currentLevel = level;

            playerModel = player.GetComponent<PlayerModel>();
            if (!playerModel)
            {
                Debug.LogError("[CherenkovMajor] PlayerModel not found!");
                return;
            }

            auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogError("[CherenkovMajor] No AuraController found on player.");
                return;
            }

            // Clone and scale behavior
            scaledWeakenBehavior = ScriptableObject.CreateInstance<AuraWeakenEffect>();
            scaledWeakenBehavior.weakenDuration = weakenBehavior.weakenDuration;
            scaledWeakenBehavior.damageMultiplier = GetScaledDamageMultiplier(level);
            scaledWeakenBehavior.sourceId = "Cherenkov Major";

            // Activar aura permanente
            auraCtrl.AddAura(auraData, scaledWeakenBehavior);

            Debug.Log($"[CherenkovMajor] ⚛️ WEAKENING AURA ACTIVE! Level {level}, Damage multiplier: {scaledWeakenBehavior.damageMultiplier:P0}, Radius: {auraData.radius}");
        }

        public override void RemoveEffect(GameObject player)
        {
            Debug.Log("[CherenkovMajor] RemoveEffect called.");
            CleanupReferences();
        }

        public override string GetDescriptionAtLevel(int level)
        {
            if (!ValidateReferences())
                return "Missing configuration data.";

            float damageMultiplier = GetScaledDamageMultiplier(level);
            float damageIncrease = (damageMultiplier - 1f) * 100f;
            return $"Permanent aura ({auraData.radius}m) weakens enemies, making them take +{damageIncrease:F0}% damage.";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && weakenBehavior != null;
        }

        private float GetScaledDamageMultiplier(int level)
        {
            // Escala el multiplicador: 1.25 → 1.30 → 1.35 → 1.40 → 1.45
            // (+25% → +45% daño)
            return weakenBehavior.damageMultiplier + 0.05f * (level - 1);
        }

        private void CleanupReferences()
        {
            Debug.Log("[CherenkovMajor] CleanupReferences - clearing all runtime state.");

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
                    Debug.LogWarning($"[CherenkovMajor] Exception while removing aura: {e.Message}");
                }
                auraCtrl = null;
            }

            // Destruir instancia runtime
            if (scaledWeakenBehavior != null)
            {
                try
                {
                    Destroy(scaledWeakenBehavior);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[CherenkovMajor] Exception while destroying scaledWeakenBehavior: {e.Message}");
                }
                scaledWeakenBehavior = null;
            }

            playerModel = null;
            currentLevel = 1;
        }
        #endregion
    }
}