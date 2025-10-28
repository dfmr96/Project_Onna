using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Neutrons Muscular Major", menuName = "Mutations/Effects/Muscular System/Neutrons Major")]
    public class NeutronsMuscularMajorEffect : StatModifierEffect
    {
        private int enemiesToKill = 9;
        private float timeToRecover = 10f;
        private void Awake()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Muscular;
            slotType = SlotType.Major;
            effectName = "Neutrons Major";
            description = $"Every {enemiesToKill} enemies eliminated gains +{timeToRecover}s life time. (max. 120s)";
            isTemporary = false;
        }


        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.SetMuscularNeutronsMajor(enemiesToKill, timeToRecover);
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
                controller.UnSetMuscularNeutronsMajor();
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
