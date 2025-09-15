using Mutations.Core.Categories;
using UnityEngine;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Gamma Nervous Minor", menuName = "Mutations/Effects/Nervous System/Gamma Minor")]
    public class GammaNervousMinorEffect : StatModifierEffect
    {
        [Header("Gamma Nervous Minor Settings")]
        [SerializeField] private float orbAttractRangeBase = 8.0f;
        [SerializeField] private float orbAttractSpeedBase = 1.5f;

        // Para trackear el level aplicado
        private int appliedLevel = 0;

        private void Awake()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Atracción Gamma Neural";
            description = "Atrae orbes de vida desde {range}m de distancia a velocidad x{speed}";
            baseValue = orbAttractRangeBase;
            upgradeMultiplier = 1.25f;
            maxLevel = 4;

            statType = StatModifierType.OrbAttractRange;
            isMultiplier = false;
            isTemporary = false;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            float attractRange = GetValueAtLevel(level);
            float attractSpeed = GetAttractSpeedAtLevel(level);

            return $"Atrae orbes de vida desde {attractRange:F1}m de distancia a velocidad x{attractSpeed:F1}";
        }

        protected override void ApplyStatModification(Player.PlayerModel playerModel, int level)
        {
            // Guardar el level aplicado
            appliedLevel = level;

            float attractRange = GetValueAtLevel(level);
            float attractSpeed = GetAttractSpeedAtLevel(level);

            // Usar el sistema de stats del proyecto
            var statContext = playerModel.StatContext;
            if (statContext != null && statContext.Target != null)
            {
                // Aplicar bonuses de atracción de orbes
                if (playerModel.StatRefs.orbAttractRange != null)
                {
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.orbAttractRange, attractRange);
                    Debug.Log($"[Gamma Nervous Minor] Applied Orb Attract Range: +{attractRange:F1}m");
                }
                else
                {
                    Debug.LogWarning("[Gamma Nervous Minor] orbAttractRange stat not found in StatReferences!");
                }

                if (playerModel.StatRefs.orbAttractSpeed != null)
                {
                    float speedBonus = attractSpeed - 1f; // Convertir multiplicador a bonus flat
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.orbAttractSpeed, speedBonus);
                    Debug.Log($"[Gamma Nervous Minor] Applied Orb Attract Speed: +{speedBonus:F1} (total x{attractSpeed:F1})");
                }
                else
                {
                    Debug.LogWarning("[Gamma Nervous Minor] orbAttractSpeed stat not found in StatReferences!");
                }

                Debug.Log($"[Gamma Nervous Minor] Applied Level {level}: Range {attractRange:F1}m, Speed x{attractSpeed:F1}");
            }
        }

        protected override void RemoveStatModification(Player.PlayerModel playerModel)
        {
            var statContext = playerModel.StatContext;
            if (statContext != null && statContext.Target != null)
            {
                // Usar el level que se aplicó
                float attractRange = GetValueAtLevel(appliedLevel);
                float attractSpeed = GetAttractSpeedAtLevel(appliedLevel);

                // Remover modificadores
                if (playerModel.StatRefs.orbAttractRange != null)
                {
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.orbAttractRange, -attractRange);
                    Debug.Log($"[Gamma Nervous Minor] Removed Orb Attract Range: -{attractRange:F1}m");
                }

                if (playerModel.StatRefs.orbAttractSpeed != null)
                {
                    float speedBonus = attractSpeed - 1f; // Mismo cálculo que al aplicar
                    statContext.Target.AddFlatBonus(playerModel.StatRefs.orbAttractSpeed, -speedBonus);
                    Debug.Log($"[Gamma Nervous Minor] Removed Orb Attract Speed: -{speedBonus:F1}");
                }

                Debug.Log($"[Gamma Nervous Minor] Effect removed (Level {appliedLevel}): Range -{attractRange:F1}m, Speed -{attractSpeed:F1}");

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
                Debug.LogWarning("[Gamma Nervous Minor] PlayerModel component not found!");
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
                Debug.LogWarning("[Gamma Nervous Minor] PlayerModel component not found!");
            }
        }

        // Métodos para obtener los valores específicos para UI/debug
        public float GetAttractRangeAtLevel(int level)
        {
            return GetValueAtLevel(level);
        }

        public float GetAttractSpeedAtLevel(int level)
        {
            return orbAttractSpeedBase * Mathf.Pow(upgradeMultiplier, level - 1);
        }
    }
}