using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Beta Nervous Minor", menuName = "Mutations/Effects/Nervous System/Beta Minor")]
    public class BetaNervousMinorEffect : StatModifierEffect
    {
        [Header("Beta Nervous Minor Settings")]
        [SerializeField] private float baseSpeedBonus = 0.2f; // incremento base (20%)

        // Para trackear el level aplicado
        private int appliedLevel = 0;

        private void Awake()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Velocidad Beta Neural";
            description = "Incrementa ligeramente la velocidad de movimiento en +{percent}%";
            baseValue = baseSpeedBonus;
            upgradeMultiplier = 1.15f;
            maxLevel = 4;
            statType = StatModifierType.MovementSpeed;
            isMultiplier = true;
            isTemporary = false;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float bonus = GetValueAtLevel(level);
            float percent = (bonus - 1f) * 100f;
            return $"Incrementa la velocidad de movimiento en +{percent:F0}%";
        }

        protected override void ApplyStatModification(Player.PlayerModel playerModel, int level)
        {
            appliedLevel = level;
            var statContext = playerModel.StatContext;

            if (statContext != null && statContext.Target != null)
            {
                if (playerModel.StatRefs.movementSpeed != null)
                {
                    float multiplier = GetValueAtLevel(level);
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.movementSpeed, multiplier);
                    Debug.Log($"[Beta Nervous Minor] Applied Movement Speed x{multiplier:F2}");
                }
                else
                {
                    Debug.LogWarning("[Beta Nervous Minor] movementSpeed stat not found in StatReferences!");
                }
            }
        }

        protected override void RemoveStatModification(Player.PlayerModel playerModel)
        {
            var statContext = playerModel.StatContext;
            if (statContext != null && statContext.Target != null)
            {
                // Usar el level que se aplicó
                float speedBonus = GetValueAtLevel(appliedLevel);

                // Remover modificadores
                if (playerModel.StatRefs.movementSpeed != null)
                {
                    // Como al aplicar sumamos, acá restamos el mismo valor
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.movementSpeed, -speedBonus);
                    Debug.Log($"[Beta Nervous Minor] Removed Movement Speed: -{speedBonus:F2}");
                }
                else
                {
                    Debug.LogWarning("[Beta Nervous Minor] movementSpeed stat not found in StatReferences!");
                }

                Debug.Log($"[Beta Nervous Minor] Effect removed (Level {appliedLevel}): Speed -{speedBonus:F2}");

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
                Debug.LogWarning("[Beta Nervous Minor] PlayerModel component not found!");
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
                Debug.LogWarning("[Beta Nervous Minor] PlayerModel component not found!");
            }
        }

        // Métodos para obtener los valores específicos para UI/debug
        public float GetSpeedMultiplierAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        
    }
}
