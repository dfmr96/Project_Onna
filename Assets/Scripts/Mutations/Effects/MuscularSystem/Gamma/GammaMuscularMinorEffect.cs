using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Mutations.Core.Categories;

namespace Mutations.Effects
{
    [CreateAssetMenu(fileName = "Gamma Muscular Minor", menuName = "Mutations/Effects/Muscular System/Gamma Minor")]
    public class GammaMuscularMinorEffect : StatModifierEffect
    {
          [Header("Bullet Modifier")]
        [SerializeField] private IgnoreLayerBulletModifierSO ignoreLayerBulletModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Gamma;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Gamma Muscular Minor";
            description = "Disparos atraviesan obstáculos del escenario, pero no enemigos adicionales.";
            statType = StatModifierType.ShootingBuff;

        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
             var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(ignoreLayerBulletModifierSO);
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(ignoreLayerBulletModifierSO);
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level)
        {
            
        }

        protected override void RemoveStatModification(PlayerModel playerModel)
        {
            
        }
    }

}



