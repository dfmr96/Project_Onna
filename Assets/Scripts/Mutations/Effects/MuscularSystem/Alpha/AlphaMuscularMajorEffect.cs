using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Mutations.Core.Categories;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Alpha Muscular Major", menuName = "Mutations/Effects/Muscular System/Alpha Major")]
    public class AlphaMuscularMajorEffect : StatModifierEffect
    {
        [Header("Alpha Muscular Major Settings")]
        [SerializeField] private float damageMultiplier = 2f;   // Daño x2
        [SerializeField] private float fireRatePenalty = 1.5f;

        [Header("Bullet Modifier")]
        [SerializeField] private OnlyTrailEffectBulletModifierSO trailEffectModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Alpha Muscular Major";
            description = "Disparos con mucho más daño pero cadencia reducida.";
            statType = StatModifierType.ShootingBuff;
            isMultiplier = true;
            isTemporary = false;
        }

        public override string GetDescriptionAtLevel(int level)
        {
            return $"Incrementa el daño x{damageMultiplier:F2}, pero reduce la cadencia a x{fireRatePenalty:F2}.";
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level)
        {
            var stats = playerModel.StatContext.Target;
            if (stats != null)
            {
                // aplicar daño
                stats.AddMultiplierBonus(playerModel.StatRefs.damage, damageMultiplier);
                Debug.Log($"[Alpha Muscular Major] Applied Damage x{damageMultiplier:F2}");

                // aplicar penalización cadencia
                stats.AddMultiplierBonus(playerModel.StatRefs.fireRate, fireRatePenalty);
                Debug.Log($"[Alpha Muscular Major] Applied FireRate x{fireRatePenalty:F2}");
            }
        }

        protected override void RemoveStatModification(PlayerModel playerModel)
        {
            var stats = playerModel.StatContext.Target;
            if (stats != null)
            {
                // revertir daño
                stats.AddMultiplierBonus(playerModel.StatRefs.damage, 1f / damageMultiplier);
                Debug.Log($"[Alpha Muscular Major] Removed Damage x{damageMultiplier:F2}");

                // revertir cadencia
                stats.AddMultiplierBonus(playerModel.StatRefs.fireRate, 1f / fireRatePenalty);
                Debug.Log($"[Alpha Muscular Major] Removed FireRate x{fireRatePenalty:F2}");
            }
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var playerModel = player.GetComponent<PlayerModel>();
            if (playerModel != null)
                ApplyStatModification(playerModel, level);


            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(trailEffectModifierSO); 
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var playerModel = player.GetComponent<PlayerModel>();
            if (playerModel != null)
                RemoveStatModification(playerModel);

            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(trailEffectModifierSO);
            }
        }
    }
}
