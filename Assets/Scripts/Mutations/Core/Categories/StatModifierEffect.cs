using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public enum StatModifierType
    {
        MovementSpeed,
        DashDistance,
        HealingMultiplier,
        HealthDrain,
        InvulnerabilityDuration,
        OrbAttractRange
    }

    public abstract class StatModifierEffect : RadiationEffect
    {
        [Header("Stat Modifier Settings")]
        [SerializeField] protected StatModifierType statType;
        [SerializeField] protected bool isMultiplier = true;
        [SerializeField] protected bool isTemporary = false;
        [SerializeField] protected float temporaryDuration = 5f;

        public StatModifierType StatType => statType;
        public bool IsMultiplier => isMultiplier;
        public bool IsTemporary => isTemporary;
        public float TemporaryDuration => temporaryDuration;

        protected float GetStatValueAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            string operation = isMultiplier ? "x" : "+";
            string temporaryText = isTemporary ? $" durante {temporaryDuration:F1}s" : "";

            return statType switch
            {
                StatModifierType.MovementSpeed => $"Velocidad de movimiento {operation}{GetStatValueAtLevel(level):F1}{temporaryText}",
                StatModifierType.DashDistance => $"Distancia de dash {operation}{GetStatValueAtLevel(level):F1}{temporaryText}",
                StatModifierType.HealingMultiplier => $"Curación de orbes {operation}{GetStatValueAtLevel(level):F1}{temporaryText}",
                StatModifierType.HealthDrain => $"Drenaje de vida {operation}{GetStatValueAtLevel(level):F1}/s{temporaryText}",
                StatModifierType.InvulnerabilityDuration => $"Invulnerabilidad {GetStatValueAtLevel(level):F1}s{temporaryText}",
                _ => $"Modifica estadística {operation}{GetStatValueAtLevel(level):F1}{temporaryText}"
            };
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var playerModel = player.GetComponent<Player.PlayerModel>();
            if (playerModel != null)
            {
                ApplyStatModification(playerModel, level);
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var playerModel = player.GetComponent<Player.PlayerModel>();
            if (playerModel != null)
            {
                RemoveStatModification(playerModel);
            }
        }

        protected abstract void ApplyStatModification(Player.PlayerModel playerModel, int level);
        protected abstract void RemoveStatModification(Player.PlayerModel playerModel);
    }
}