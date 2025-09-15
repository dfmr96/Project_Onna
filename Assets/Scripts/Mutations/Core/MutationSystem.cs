using UnityEngine;

namespace Mutations.Core
{
    [System.Serializable]
    public class MutationSystem
    {
        [SerializeField] private SystemType systemType;
        [SerializeField] private MutationSlot majorSlot;
        [SerializeField] private MutationSlot minorSlot;

        public SystemType SystemType => systemType;
        public MutationSlot MajorSlot => majorSlot;
        public MutationSlot MinorSlot => minorSlot;

        public MutationSystem(SystemType type)
        {
            systemType = type;
            majorSlot = new MutationSlot(SlotType.Major);
            minorSlot = new MutationSlot(SlotType.Minor);
        }

        public SlotType GetTargetSlot(MutationType radiation)
        {
            if (majorSlot.CanAssignRadiation(radiation))
                return SlotType.Major;

            if (minorSlot.CanAssignRadiation(radiation))
                return SlotType.Minor;

            return SlotType.Major;
        }

        public bool CanReceiveRadiation(MutationType radiation)
        {
            return majorSlot.CanAssignRadiation(radiation) || minorSlot.CanAssignRadiation(radiation);
        }

        public void AssignRadiation(MutationType radiation, RadiationEffect effect, GameObject player)
        {
            SlotType targetSlot = GetTargetSlot(radiation);

            if (targetSlot == SlotType.Major)
                majorSlot.AssignRadiation(radiation, effect, player);
            else
                minorSlot.AssignRadiation(radiation, effect, player);
        }

        public void ClearAllMutations(GameObject player)
        {
            majorSlot.Clear(player);
            minorSlot.Clear(player);
        }
    }
}