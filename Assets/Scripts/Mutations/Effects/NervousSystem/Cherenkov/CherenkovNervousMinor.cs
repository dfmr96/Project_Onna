using Mutations.Core.Categories;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Chrenkov Nervous Minor", menuName = "Mutations/Effects/Nervous System/Chrenkov Minor")]
    public class CherenkovNervousMinor : StatModifierEffect
    {
        [Header("Bullet Modifier")]
        [SerializeField] private MarkerOrbsBulletModifierSO markerModifierSO;

        private void Awake()
        {
            radiationType = MutationType.Cherenkov;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Cherenkov Minor";
            description = "Marked enemies drop an extra vital orb upon death.";
            isTemporary = false;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.AddBulletModifier(markerModifierSO);
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.RemoveBulletModifier(markerModifierSO);
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
