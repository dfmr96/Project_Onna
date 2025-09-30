using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects.NervousSystem
{
    [CreateAssetMenu(fileName = "Microwaves Nervous Minor", menuName = "Mutations/Effects/Nervous System/Microwaves Minor")]
    public class MicrowavesNervousMinorEffect : StatModifierEffect
    {
        [Header("Microwaves Minor Settings")]

        [SerializeField] private float minBurnDuration = 1f;
        [SerializeField] private float maxBurnDuration = 2f;
        [SerializeField] private float damagePerTick = 1f;

  
        private void Awake()
        {
            radiationType = MutationType.Microwaves;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Microwaves Minor";
            description = $"Cada Disparos aplican quemadura leve de +{minBurnDuration}-+{maxBurnDuration} segundos.";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                float duration = Random.Range(minBurnDuration, maxBurnDuration);
                controller.SetMicrowavesMinor(true, duration, damagePerTick);
                Debug.Log("[MinorEffect] Microwaves Minor activada en Player");
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.SetMicrowavesMinor(false);
                Debug.Log("[MinorEffect] Microwaves Minor desactivada en Player");
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}