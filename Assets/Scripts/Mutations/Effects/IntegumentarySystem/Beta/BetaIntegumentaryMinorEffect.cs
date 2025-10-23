using Mutations.Core;
using UnityEngine;

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Beta Integumentary Minor", menuName = "Mutations/Effects/Integumentary System/Beta Minor")]
    public class BetaIntegumentaryMinorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraRandomSlowEffect randomSlowBehavior;

        private AuraRandomSlowEffect runtimeBehavior;

        private void OnEnable()
        {
            radiationType = MutationType.Beta;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Minor;
            effectName = "Beta Integumentary Minor";
            description = "Occasionally slows a random subset of nearby enemies.";
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            if (!ValidateReferences())
            {
                Debug.LogError($"[BetaMinor] Missing required references. Check auraData and randomSlowBehavior assignments.");
                return;
            }

            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogWarning($"[BetaMinor] AuraController missing on player (level {level}).");
                return;
            }

            // Crear comportamiento runtime escalado
            runtimeBehavior = ScriptableObject.CreateInstance<AuraRandomSlowEffect>();
            runtimeBehavior.speedMultiplier = GetScaledSpeedMultiplier(level);
            runtimeBehavior.duration = GetScaledDuration(level);
            runtimeBehavior.affectedFraction = GetScaledAffectedFraction(level);

            auraCtrl.AddAura(auraData, runtimeBehavior);

            Debug.Log($"[BetaMinor] Random slow aura applied at level {level} (affects {runtimeBehavior.affectedFraction:P0} of enemies).");
        }

        public override void RemoveEffect(GameObject player)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (auraCtrl && auraData != null)
                auraCtrl.RemoveAura(auraData.auraId);

            // Limpiar instancia runtime para evitar memory leak
            if (runtimeBehavior != null)
            {
                Destroy(runtimeBehavior);
                runtimeBehavior = null;
            }
        }

        public override string GetDescriptionAtLevel(int level)
        {
            if (!ValidateReferences())
                return "Missing configuration data.";

            float slowPct = (1f - GetScaledSpeedMultiplier(level)) * 100f;
            float fracPct = GetScaledAffectedFraction(level) * 100f;
            float duration = GetScaledDuration(level);
            return $"Every {auraData.tickRate:F1}s, slows ~{fracPct:F0}% of nearby enemies by {slowPct:F0}% for {duration:F1}s.";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && randomSlowBehavior != null;
        }

        private float GetScaledSpeedMultiplier(int level)
        {
            return Mathf.Clamp(randomSlowBehavior.speedMultiplier - 0.05f * (level - 1), 0.4f, 0.95f);
        }

        private float GetScaledDuration(int level)
        {
            return randomSlowBehavior.duration + 0.2f * (level - 1);
        }

        private float GetScaledAffectedFraction(int level)
        {
            return Mathf.Clamp01(randomSlowBehavior.affectedFraction + 0.1f * (level - 1));
        }
        #endregion
    }
}
