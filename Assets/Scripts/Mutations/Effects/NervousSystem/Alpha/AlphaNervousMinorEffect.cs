
using UnityEngine;
using Player;
using Enemy.Spawn;
using Mutations.Core.Categories;

namespace Mutations.Effects.NervousSystem
{

    [CreateAssetMenu(fileName = "Alpha Minor", menuName = "Mutations/Effects/Nervous System/Alpha Minor")]
    public class AlphaNervousMinorEffect : StatModifierEffect
    {
        [Header("Alpha Minor Settings")]
        [SerializeField] private float invulnerabilityDuration = 0.1f; // Duración muy breve, sin cooldown

        private void Awake()
        {
            radiationType = MutationType.Alpha;
            systemType = SystemType.Nerve;
            slotType = SlotType.Minor;
            effectName = "Alpha Mini-Invulnerabilidad";
            description = $"Cada orbe recogido otorga un mini-burst de invulnerabilidad de {invulnerabilityDuration} segundos";
            isTemporary = true;
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null)
            {
                Debug.LogWarning("[Alpha Minor] PlayerControllerEffect no encontrado en el player.");
                return;
            }

            controllerEffect.SetAlphaMinor(true, invulnerabilityDuration);
            Debug.Log("[Alpha Minor] Activada.");
        }

        public override void RemoveEffect(GameObject player)
        {
            var controllerEffect = player.GetComponent<PlayerControllerEffect>();
            if (controllerEffect == null) return;

            controllerEffect.SetAlphaMinor(false);
            Debug.Log("[Alpha Minor] Desactivada.");
        }



        protected override void ApplyStatModification(PlayerModel playerModel, int level)
        {
            throw new System.NotImplementedException();
        }

        protected override void RemoveStatModification(PlayerModel playerModel)
        {
            throw new System.NotImplementedException();
        }
    }




}