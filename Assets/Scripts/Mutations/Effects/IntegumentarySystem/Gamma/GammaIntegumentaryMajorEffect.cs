using Mutations.Core;
using UnityEngine;

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Gamma Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Gamma Major")]
    public class GammaIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Aura References")]
        [SerializeField] private AuraData auraData;
        [SerializeField] private AuraDamageEffect behavior;

        private void OnEnable()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Gamma Integumentary Major";
            description = "Creates a radioactive aura that deals continuous damage to nearby enemies.";
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (!auraCtrl)
            {
                Debug.LogWarning("AuraController component not found on player.");
                return;
            }

            // Scale damage based on the current upgrade level
            var scaledBehavior = ScriptableObject.CreateInstance<AuraDamageEffect>();
            scaledBehavior.damagePerSecond = behavior.damagePerSecond * GetValueAtLevel(level);

            auraCtrl.AddAura(auraData, scaledBehavior);
            Debug.Log($"[Gamma Aura] Level {level} aura applied ({scaledBehavior.damagePerSecond:F1} dmg/s).");
        }

        public override void RemoveEffect(GameObject player)
        {
            var auraCtrl = player.GetComponentInChildren<AuraController>();
            if (auraCtrl)
                auraCtrl.RemoveAura(auraData.auraId);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float dmg = behavior.damagePerSecond * GetValueAtLevel(level);
            return $"Creates a radioactive aura that deals {dmg:F1} damage/s within a radius of {auraData.radius}m.";
        }
    }
}
