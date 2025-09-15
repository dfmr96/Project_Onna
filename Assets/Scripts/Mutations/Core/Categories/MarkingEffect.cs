using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public enum MarkingTrigger
    {
        OnShot,
        OnMelee,
        OnGrenade,
        OnAura,
        OnDamageReceived
    }

    public enum MarkingEffectType
    {
        IncreaseDamageReceived,
        DecreaseDamageDealt,
        SlowMovement,
        IncreaseOrbDrop
    }

    public abstract class MarkingEffect : RadiationEffect
    {
        [Header("Marking Settings")]
        [SerializeField] protected MarkingTrigger triggerType;
        [SerializeField] protected MarkingEffectType markEffect;
        [SerializeField] protected float markDuration = 5f;
        [SerializeField] protected float markIntensity = 0.25f;
        [SerializeField] protected bool canStack = false;
        [SerializeField] protected int maxStacks = 3;

        public MarkingTrigger TriggerType => triggerType;
        public MarkingEffectType MarkEffect => markEffect;
        public float MarkDuration => markDuration;
        public float MarkIntensity => markIntensity;
        public bool CanStack => canStack;
        public int MaxStacks => maxStacks;

        protected float GetMarkIntensityAtLevel(int level)
        {
            return markIntensity * GetValueAtLevel(level);
        }

        protected float GetMarkDurationAtLevel(int level)
        {
            return markDuration * (1f + (level - 1) * 0.2f);
        }

        protected int GetMaxStacksAtLevel(int level)
        {
            return canStack ? maxStacks + (level - 1) : 1;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            string triggerText = triggerType switch
            {
                MarkingTrigger.OnShot => "Disparos",
                MarkingTrigger.OnMelee => "Ataques melee",
                MarkingTrigger.OnGrenade => "Granadas",
                MarkingTrigger.OnAura => "Aura",
                MarkingTrigger.OnDamageReceived => "Al recibir daño",
                _ => "Acciones"
            };

            string effectText = markEffect switch
            {
                MarkingEffectType.IncreaseDamageReceived => $"reciben +{GetMarkIntensityAtLevel(level) * 100:F0}% daño",
                MarkingEffectType.DecreaseDamageDealt => $"infligen -{GetMarkIntensityAtLevel(level) * 100:F0}% daño",
                MarkingEffectType.SlowMovement => $"se mueven {GetMarkIntensityAtLevel(level) * 100:F0}% más lento",
                MarkingEffectType.IncreaseOrbDrop => $"sueltan orbes extra",
                _ => "sufren efecto especial"
            };

            string stackText = canStack ? $" (máx. {GetMaxStacksAtLevel(level)} stacks)" : "";

            return $"{triggerText} marcan enemigos: {effectText} durante {GetMarkDurationAtLevel(level):F1}s{stackText}";
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // TODO: Implementar integración con sistema de marcado del juego
            Debug.Log($"[MarkingEffect] Applied {EffectName} at level {level}");

            RegisterMarkingEffect(player, level);
        }

        public override void RemoveEffect(GameObject player)
        {
            // TODO: Implementar remoción de efecto de marcado
            Debug.Log($"[MarkingEffect] Removed {EffectName}");

            UnregisterMarkingEffect(player);
        }

        protected virtual void RegisterMarkingEffect(GameObject player, int level) { }
        protected virtual void UnregisterMarkingEffect(GameObject player) { }
    }
}