using UnityEngine;

namespace Mutations.Core
{
    [System.Serializable]
    public class MutationSlot
    {
        [SerializeField] private SlotType slotType;
        [SerializeField] private MutationType? radiationType;
        [SerializeField] private int upgradeLevel = 0;
        [SerializeField] private RadiationEffect activeEffect;

        public SlotType SlotType => slotType;
        public MutationType? RadiationType => radiationType;
        public int UpgradeLevel => upgradeLevel;
        public RadiationEffect ActiveEffect => activeEffect;
        public bool IsEmpty => radiationType == null;

        public MutationSlot(SlotType type)
        {
            slotType = type;
        }

        public bool CanAssignRadiation(MutationType radiation)
        {
            return IsEmpty || (radiationType == radiation && upgradeLevel < 4);
        }

        public void AssignRadiation(MutationType radiation, RadiationEffect effect, GameObject player)
        {
            if (IsEmpty)
            {
                radiationType = radiation;
                upgradeLevel = 1;
                activeEffect = effect;
                effect.ApplyEffect(player, upgradeLevel);
            }
            else if (radiationType == radiation && upgradeLevel < 4)
            {
                upgradeLevel++;
                if (activeEffect != null)
                {
                    activeEffect.RemoveEffect(player);
                    activeEffect.ApplyEffect(player, upgradeLevel);
                }
            }
        }

        public void Clear(GameObject player)
        {
            if (activeEffect != null)
            {
                activeEffect.RemoveEffect(player);
            }

            radiationType = null;
            upgradeLevel = 0;
            activeEffect = null;
        }
    }
}