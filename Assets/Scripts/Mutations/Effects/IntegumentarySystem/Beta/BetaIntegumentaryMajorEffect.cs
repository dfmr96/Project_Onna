using Mutations.Core;
using UnityEngine;

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Beta Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Beta Major")]
    public class BetaIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Aura")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraSlowEffect behavior;

        private AuraSlowEffect runtimeBehavior;

        private void OnEnable()
        {
            radiationType = MutationType.Beta;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Beta Integumentary Major";
            description = "Creates a friction field that continuously slows enemies around the player.";
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogWarning("[BetaIntegumentaryMajor] AuraController not found on player.");
                return;
            }

            // Escalar el slow con el nivel
            runtimeBehavior = ScriptableObject.CreateInstance<AuraSlowEffect>();
            runtimeBehavior.speedMultiplier = Mathf.Clamp(behavior.speedMultiplier - 0.05f * (level - 1), 0.4f, 0.95f);
            runtimeBehavior.duration = behavior.duration;

            auraCtrl.AddAura(auraData, runtimeBehavior);

            Debug.Log($"[BetaIntegumentaryMajor] Aura active (slow {1f - runtimeBehavior.speedMultiplier:P0}).");
        }

        public override void RemoveEffect(GameObject player)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (auraCtrl)
                auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float slow = (1f - Mathf.Clamp(behavior.speedMultiplier - 0.05f * (level - 1), 0.4f, 0.95f)) * 100f;
            return $"Generates a friction field slowing nearby enemies by {slow:F0}% within {auraData.radius:F1}m.";
        }
    }
}
