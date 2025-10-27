using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Neutrons Nervous Minor", menuName = "Mutations/Effects/Nervous System/Neutrons Minor")]
    public class NeutronsNervousMinorEffect : StatModifierEffect
    {
        [Header("Neutrons Minor Settings")]
        private float extraTime = 1.5f;

        private void Awake()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Neutrons Minor";
            description = $"Each vital orb collected adds +{extraTime} seconds of vital time.";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null) return;

            controllerEffect.SetNeutronsEffect(true, extraTime);
        }

        public override void RemoveEffect(GameObject player)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null) return;

            controllerEffect.SetNeutronsEffect(false, 0);
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }

}