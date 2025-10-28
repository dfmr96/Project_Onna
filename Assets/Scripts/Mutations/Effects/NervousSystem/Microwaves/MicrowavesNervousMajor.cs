using Mutations.Core.Categories;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mutations.Effects.NervousSystem 
{
    [CreateAssetMenu(fileName = "Microwaves Nervous Major", menuName = "Mutations/Effects/Nervous System/Microwaves Major")]
    public class MicrowavesNervousMajor : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private BurnBulletModifierSO burnBulletModifierSO;
        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Nerve;
            slotType = SlotType.Major;
            effectName = "Microwaves Major";
            description = "After successfully performing the skillcheck, the next charger will apply a burn.";
            statType = StatModifierType.ShootingBuff;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.SetSkillCheckFullClipBurn(burnBulletModifierSO);
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.UnSetSkillCheckFullClipBurn();
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }

        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
