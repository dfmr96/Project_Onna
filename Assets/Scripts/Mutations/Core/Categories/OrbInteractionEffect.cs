using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public enum OrbInteractionType
    {
        Attraction,
        ExtraSpawn,
        HealingBonus,
        SpecialTrigger
    }

    public abstract class OrbInteractionEffect : RadiationEffect
    {
        [Header("Orb Interaction Settings")]
        [SerializeField] protected OrbInteractionType interactionType;
        [SerializeField] protected float attractionRange = 5f;
        [SerializeField] protected int extraOrbsCount = 1;
        [SerializeField] protected float orbSpawnChance = 1f;

        public OrbInteractionType InteractionType => interactionType;
        public float AttractionRange => attractionRange;
        public int ExtraOrbsCount => extraOrbsCount;
        public float OrbSpawnChance => orbSpawnChance;

        protected float GetInteractionValueAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        protected float GetAttractionRangeAtLevel(int level)
        {
            return attractionRange * GetValueAtLevel(level);
        }

        protected int GetExtraOrbsAtLevel(int level)
        {
            return Mathf.RoundToInt(extraOrbsCount * GetValueAtLevel(level));
        }

        public override string GetDescriptionAtLevel(int level)
        {
            return interactionType switch
            {
                OrbInteractionType.Attraction => $"Atrae orbes desde {GetAttractionRangeAtLevel(level):F1}m",
                OrbInteractionType.ExtraSpawn => $"Enemigos sueltan {GetExtraOrbsAtLevel(level)} orbes extra",
                OrbInteractionType.HealingBonus => $"Orbes curan {GetInteractionValueAtLevel(level):F1}x más",
                OrbInteractionType.SpecialTrigger => $"Orbes activan efecto especial",
                _ => "Modifica interacción con orbes"
            };
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            // TODO: Implementar integración con sistema de orbes del juego
            Debug.Log($"[OrbInteractionEffect] Applied {EffectName} at level {level}");

            RegisterOrbInteraction(player, level);
        }

        public override void RemoveEffect(GameObject player)
        {
            // TODO: Implementar remoción de efecto de orbes
            Debug.Log($"[OrbInteractionEffect] Removed {EffectName}");

            UnregisterOrbInteraction(player);
        }

        protected virtual void RegisterOrbInteraction(GameObject player, int level) { }
        protected virtual void UnregisterOrbInteraction(GameObject player) { }
    }
}