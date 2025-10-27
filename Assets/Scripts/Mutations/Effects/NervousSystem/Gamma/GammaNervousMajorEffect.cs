using Mutations.Core.Categories;
using UnityEngine;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Gamma Nervous Major", menuName = "Mutations/Effects/Nervous System/Gamma Major")]
    public class GammaNervousMajorEffect : StatModifierEffect
    {
        [Header("Gamma Nervous Major Settings")]
        private float healingMultiplierBase = 1.5f;
        private float healthDrainIncrease = 0.5f;

        // Para trackear el level aplicado
        private int appliedLevel = 0;

        private void Awake()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Nerve;
            slotType = SlotType.Major;
            effectName = "Radiación Gamma Neural";
            description = $"Increases the healing of vital orbs by +{healingMultiplierBase}% but the drain of vital time rises to +{healthDrainIncrease}/s.";
            baseValue = healingMultiplierBase;
            upgradeMultiplier = 1.3f;
            maxLevel = 4;

            statType = StatModifierType.HealingMultiplier;
            isMultiplier = true;
            isTemporary = false;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float healingMult = GetValueAtLevel(level);
            float drainIncrease = healthDrainIncrease * level;

            return $"Aumenta curación de orbes x{healingMult:F1} pero incrementa drenaje de vida +{drainIncrease:F1}/s";
        }

        protected override void ApplyStatModification(Player.PlayerModel playerModel, int level)
        {
            // Guardar el level aplicado
            appliedLevel = level;

            float healingMultiplier = GetValueAtLevel(level);
            float drainIncrease = healthDrainIncrease * level;

            // Usar el sistema de stats del proyecto
            var statContext = playerModel.StatContext;
            if (statContext != null && statContext.Target != null)
            {
                // Aplicar multiplicador de curación (bonus adicional, no total)
                float bonusMultiplier = healingMultiplier - 1f; // Convertir a bonus (1.5 -> 0.5)
                statContext.Target.AddFlatBonus(playerModel.StatRefs.healingMultiplier, bonusMultiplier);

                // Aplicar incremento de drenaje (bonus flat)
                statContext.Target.AddFlatBonus(playerModel.StatRefs.passiveDrainRate, drainIncrease);

                Debug.Log($"[Gamma Nervous Major] Applied Level {level}: Healing x{healingMultiplier:F1}, Drain +{drainIncrease:F1}/s");
            }
        }

        protected override void RemoveStatModification(Player.PlayerModel playerModel)
        {
            var statContext = playerModel.StatContext;
            if (statContext != null && statContext.Target != null)
            {
                // Usar el level que se aplicó, no level 1
                float healingMultiplier = GetValueAtLevel(appliedLevel);
                float drainIncrease = healthDrainIncrease * appliedLevel;

                // Remover modificadores (valores negativos del mismo que se aplicó)
                float bonusMultiplier = healingMultiplier - 1f; // Mismo cálculo que al aplicar
                statContext.Target.AddFlatBonus(playerModel.StatRefs.healingMultiplier, -bonusMultiplier);
                statContext.Target.AddFlatBonus(playerModel.StatRefs.passiveDrainRate, -drainIncrease);

                Debug.Log($"[Gamma Nervous Major] Effect removed (Level {appliedLevel}): Healing -{bonusMultiplier:F1}, Drain -{drainIncrease:F1}/s");

                // Reset level
                appliedLevel = 0;
            }
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var playerModel = player.GetComponent<Player.PlayerModel>();
            if (playerModel != null)
            {
                ApplyStatModification(playerModel, level);
            }
            else
            {
                Debug.LogWarning("[Gamma Nervous Major] PlayerModel component not found!");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var playerModel = player.GetComponent<Player.PlayerModel>();
            if (playerModel != null)
            {
                RemoveStatModification(playerModel);
            }
            else
            {
                Debug.LogWarning("[Gamma Nervous Major] PlayerModel component not found!");
            }
        }

        // Método para obtener los valores específicos para UI/debug
        public float GetHealingMultiplierAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        public float GetHealthDrainIncreaseAtLevel(int level)
        {
            return healthDrainIncrease * level;
        }
    }
}