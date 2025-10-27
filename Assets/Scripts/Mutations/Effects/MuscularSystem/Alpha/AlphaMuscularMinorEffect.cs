using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Mutations.Core.Categories;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Alpha Muscular Minor", menuName = "Mutations/Effects/Muscular System/Alpha Minor")]
    public class AlphaMuscularMinorEffect : StatModifierEffect
    {
        [Header("Alpha Muscular Minor Settings")]
        private float damageBonus = 0.5f; // +50% daño
        private int maxPenetration = 2;
        private int bonusToApply;

        [Header("Bullet Modifier")]
        [SerializeField] private OnlyTrailEffectBulletModifierSO trailEffectModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Alpha Muscular Minor";
            description = $"Increases the damage of each shot by +50%, but enemy penetration is reduced to a maximum of {maxPenetration} enemies.";
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

                // aumentar daño
                stats.AddMultiplierBonus(playerModel.StatRefs.damage, damageBonus);

                //stats.AddFlatBonus(playerModel.StatRefs.bulletMaxPenetration, maxPenetration);

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

                // aumentar daño
                stats.AddMultiplierBonus(playerModel.StatRefs.damage, 1f / damageBonus);

                stats.AddFlatBonus(playerModel.StatRefs.bulletMaxPenetration, -bonusToApply);

            }
        }
    }
}

