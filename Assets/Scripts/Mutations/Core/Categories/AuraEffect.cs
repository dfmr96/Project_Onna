using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public abstract class AuraEffect : RadiationEffect
    {
        [Header("Aura Settings")]
        [SerializeField] protected float auraRadius = 3f;
        [SerializeField] protected bool isContinuous = true;
        [SerializeField] protected float intervalDuration = 2f;
        [SerializeField] protected LayerMask enemyLayers = -1;

        public float AuraRadius => auraRadius;
        public bool IsContinuous => isContinuous;
        public float IntervalDuration => intervalDuration;
        public LayerMask EnemyLayers => enemyLayers;

        protected float GetAuraValueAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        protected float GetAuraRadiusAtLevel(int level)
        {
            return auraRadius * (1f + (level - 1) * 0.1f);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            string continuousText = isContinuous ? "continuo" : $"cada {intervalDuration:F1}s";
            return $"Aura de {GetAuraRadiusAtLevel(level):F1}m que aplica efecto {continuousText}";
        }

        protected abstract void ApplyAuraEffect(Collider[] targets, int level);
        protected virtual bool ShouldAffectTarget(Collider target) => true;

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // TODO: Implementar integración con sistema de auras del juego
            Debug.Log($"[AuraEffect] Applied {EffectName} at level {level}");

            // Por ahora solo registramos el efecto
            RegisterAuraEffect(player, level);
        }

        public override void RemoveEffect(GameObject player)
        {
            // TODO: Implementar remoción de aura del sistema del juego
            Debug.Log($"[AuraEffect] Removed {EffectName}");

            UnregisterAuraEffect(player);
        }

        protected virtual void RegisterAuraEffect(GameObject player, int level) { }
        protected virtual void UnregisterAuraEffect(GameObject player) { }
    }
}