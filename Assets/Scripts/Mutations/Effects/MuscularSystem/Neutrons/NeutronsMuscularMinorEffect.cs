using UnityEngine;
using Mutations.Core.Categories;
using Player;

namespace Mutations.Effects 
{
    [CreateAssetMenu(fileName = "Neutrons Muscular Minor", menuName = "Mutations/Effects/Muscular System/Neutrons Minor")]
    public class NeutronsMuscularMinorEffect : StatModifierEffect
    {
        private int enemiesToHit = 8;
        private float timeToRecover = 3f;

        [Header("Bullet Modifier")]
        [SerializeField] private CounterBulletModifierSO counterModifierSO;
        private void Awake()
        {
            radiationType = MutationType.Neutrons;
            systemType = SystemType.Muscular;
            slotType = SlotType.Minor;
            effectName = "Neutrons Minor";
            description = $"Each {enemiesToHit} successful shots without missing, gains +{timeToRecover}s of vital time.";
            isTemporary = false;
        }


        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.AddBulletModifier(counterModifierSO);
                controller.SetMuscularNeutronsMinor(enemiesToHit, timeToRecover);
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var controller = player.GetComponent<PlayerControllerEffect>();
            if (controller != null)
            {
                controller.RemoveBulletModifier(counterModifierSO);
                controller.UnSetMuscularNeutronsMinor();
            }
        }

        protected override void ApplyStatModification(PlayerModel playerModel, int level) { }
        protected override void RemoveStatModification(PlayerModel playerModel) { }
    }
}
