using Mutations.Core;
using UnityEngine;

namespace Mutations.Effects.IntegumentarySystem
{
    [CreateAssetMenu(fileName = "Gamma Integumentary Major", menuName = "Mutations/Effects/Integumentary System/Gamma Major")]
    public class GammaIntegumentaryMajorEffect : RadiationEffect
    {
        [Header("Gamma Integumentary Major Settings")]
        [SerializeField] private float damage = 5f;
        [SerializeField] private float auraRadius = 2f;
        [SerializeField] private float tickRate = 0.5f;

        private void Awake()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Integumentary;
            slotType = SlotType.Major;
            effectName = "Gamma Integumentary Major";
            description = "Radial aura deals {value} damage/s to nearby enemies";
            baseValue = damage;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            float dmg = GetValueAtLevel(level);
            // TODO: Implementar aura de daño radial con el sistema del juego
            Debug.Log($"[Gamma Integumentary Major] Applied level {level}: {dmg:F1} damage/s in {auraRadius}m radius");
        }

        public override void RemoveEffect(GameObject player)
        {
            // TODO: Implementar remoción de aura de daño
            Debug.Log("[Gamma Integumentary Major] Effect removed");
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float dmg = GetValueAtLevel(level);
            return $"Radial aura deals {dmg:F1} damage/s to enemies within {auraRadius}m";
        }
    }
}