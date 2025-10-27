using Mutations.Core.Categories;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mutations.Effects.NervousSystem 
{
    [CreateAssetMenu(fileName = "Microwaves Nervous Minor", menuName = "Mutations/Effects/Nervous System/Microwaves Minor")]
    public class MicrowavesNervousMinor : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private BurnBulletModifierSO burnBulletModifierSO;
        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Microwaves Minor";
            description = "After successfully performing the skillcheck, the next bullet will apply a burn";
            statType = StatModifierType.ShootingBuff;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.SetSkillCheckOneShotBurn(burnBulletModifierSO);
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.UnSetSkillCheckOneShotBurn();
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }

        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
