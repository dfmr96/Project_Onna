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
            runtimeBehavior.slowAmount = GetScaledSlowAmount(level);
            runtimeBehavior.duration = GetScaledDuration(level);
            runtimeBehavior.affectedFraction = GetScaledAffectedFraction(level);
            runtimeBehavior.sourceId = "Beta Minor";

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

            float slowPct = GetScaledSlowAmount(level) * 100f;
            float fracPct = GetScaledAffectedFraction(level) * 100f;
            float duration = GetScaledDuration(level);
            return $"Every {auraData.tickRate:F1}s, slows ~{fracPct:F0}% of nearby enemies by {slowPct:F0}% for {duration:F1}s.";
        }

        #region Helper Methods
        private bool ValidateReferences()
        {
            return auraData != null && randomSlowBehavior != null;
        }

        private float GetScaledSlowAmount(int level)
        {
            // Escala slowAmount: 0.25 → 0.30 → 0.35 → 0.40 → 0.45
            // (25% → 45% speed reduction)
            return Mathf.Clamp(randomSlowBehavior.slowAmount + 0.05f * (level - 1), 0.25f, 0.6f);
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
