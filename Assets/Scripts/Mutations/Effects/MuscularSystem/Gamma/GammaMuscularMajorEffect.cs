using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Mutations.Core.Categories;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Gamma Muscular Major", menuName = "Mutations/Effects/Muscular System/Gamma Major")]
    public class GammaMuscularMajorEffect : StatModifierEffect
    {
        [Header("Gamma Muscular Major Settings")]
        private int maxPenetration = 5;
        private int bonusToApply;

        [Header("Bullet Modifier")]
        [SerializeField] private OnlyTrailEffectBulletModifierSO trailEffectModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Gamma Muscular Major";
            description = $"Shots gain high penetration, causing them to pass through a maximum of {maxPenetration} enemies.";
            statType = StatModifierType.ShootingBuff;

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

        protected override void ApplyStatModification(PlayerModel playerModel, int level)
        {
            var stats = playerModel.StatContext.Target;

            if (stats != null)
            {
                if (playerModel == null) return;

                int basePenetrationValue = playerModel.BulletMaxPenetration;
                bonusToApply = maxPenetration - basePenetrationValue;
                stats.AddFlatBonus(playerModel.StatRefs.bulletMaxPenetration, bonusToApply);

            }
        }

        protected override void RemoveStatModification(PlayerModel playerModel)
        {
            var stats = playerModel.StatContext.Target;

            if (stats != null)
            {
                if (playerModel == null) return;

                stats.AddFlatBonus(playerModel.StatRefs.bulletMaxPenetration, -bonusToApply);

            }
        }
    }

}


