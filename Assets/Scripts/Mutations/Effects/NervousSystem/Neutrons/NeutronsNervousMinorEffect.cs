using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Neutrons Minor", menuName = "Mutations/Effects/Vital System/Neutrons Minor")]
    public class NeutronsNervousMinorEffect : StatModifierEffect
    {
        [Header("Neutrons Minor Settings")]
        [SerializeField] private float extraTime = 2f;

        private void Awake()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Neutrons Minor";
            description = $"Cada orbe vital recogido añade +{extraTime} segundos de tiempo vital.";
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