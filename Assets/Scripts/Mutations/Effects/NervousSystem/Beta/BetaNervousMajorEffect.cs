using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Beta Nervous Major", menuName = "Mutations/Effects/Nervous System/Beta Major")]
    public class BetaNervousMajorEffect : StatModifierEffect
    {
        [Header("Beta Nervous Major Settings")]
        private float baseSpeedBonus = 0.4f;
        private float baseDashBonus = 0.4f;

        private int appliedLevel = 0;

        private void Awake()
        {
            radiationType = MutationType.Beta;
            systemType = SystemType.Nerve;
            slotType = SlotType.Major;
            effectName = "Velocidad Beta Avanzada";
            description = $"Increases movement speed by +{baseSpeedBonus}% and dash distance by +{baseDashBonus}%.";
            baseValue = baseSpeedBonus;
            upgradeMultiplier = 1.15f;
            maxLevel = 4;
            statType = StatModifierType.MovementSpeed; // stat principal (por compatibilidad)
            isMultiplier = true;
            isTemporary = false;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float speedPercent = (GetValueAtLevel(level) - 1f) * 100f;
            float dashPercent = (GetDashMultiplierAtLevel(level) - 1f) * 100f;
            return $"Incrementa la velocidad en +{speedPercent:F0}% y el dash en +{dashPercent:F0}%";
        }

        protected override void ApplyStatModification(Player.PlayerModel playerModel, int level)
        {
            appliedLevel = level;
            var statContext = playerModel.StatContext;

            if (statContext != null && statContext.Target != null)
            {
                // Movimiento
                if (playerModel.StatRefs.movementSpeed != null)
                {
                    float speedMult = GetValueAtLevel(level);
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.movementSpeed, speedMult);
                    Debug.Log($"[Beta Nervous Major] Applied Movement Speed x{speedMult:F2}");
                }

                // Dash
                if (playerModel.StatRefs.dashDistance != null)
                {
                    float dashMult = GetDashMultiplierAtLevel(level);
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.dashDistance, dashMult);
                    Debug.Log($"[Beta Nervous Major] Applied Dash Distance x{dashMult:F2}");
                }
            }
        }

        protected override void RemoveStatModification(Player.PlayerModel playerModel)
        {
            var statContext = playerModel.StatContext;
            if (statContext != null && statContext.Target != null)
            {
                // Revertir velocidad
                if (playerModel.StatRefs.movementSpeed != null)
                {
                    float speedMult = GetValueAtLevel(appliedLevel);
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.movementSpeed, -speedMult);
                    Debug.Log($"[Beta Nervous Major] Removed Movement Speed: -{speedMult:F2}");
                }

                // Revertir dash
                if (playerModel.StatRefs.dashDistance != null)
                {
                    float dashMult = GetDashMultiplierAtLevel(appliedLevel);
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.dashDistance, -dashMult);
                    Debug.Log($"[Beta Nervous Major] Removed Dash Distance: -{dashMult:F2}");
                }

                appliedLevel = 0;
            }
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var playerModel = player.GetComponent<Player.PlayerModel>();
            if (playerModel != null)
                ApplyStatModification(playerModel, level);
        }

        public override void RemoveEffect(GameObject player)
        {
            var playerModel = player.GetComponent<Player.PlayerModel>();
            if (playerModel != null)
                RemoveStatModification(playerModel);
        }

        public float GetDashMultiplierAtLevel(int level)
        {
            return baseDashBonus * Mathf.Pow(upgradeMultiplier, level - 1);
        }
    }
}
