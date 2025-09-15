using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mutations.Core
{
    [System.Serializable]
    public class MutationOption
    {
        public MutationType radiationType;
        public SystemType targetSystem;
        public SlotType targetSlot;
        public string description;
        public Sprite icon;
        public bool isUpgrade;
        public int newLevel;

        public MutationOption(MutationType radiation, SystemType system, SlotType slot, string desc, Sprite ico, bool upgrade = false, int level = 1)
        {
            radiationType = radiation;
            targetSystem = system;
            targetSlot = slot;
            description = desc;
            icon = ico;
            isUpgrade = upgrade;
            newLevel = level;
        }
    }

    public class MutationSelectionService
    {
        private MutationManager manager;
        private RadiationEffectFactory factory;

        public MutationSelectionService(MutationManager mutationManager, RadiationEffectFactory effectFactory)
        {
            manager = mutationManager;
            factory = effectFactory;
        }

        public List<MutationOption> GenerateSelectionOptions(int count = 3)
        {
            var options = new List<MutationOption>();
            var allRadiations = System.Enum.GetValues(typeof(MutationType)).Cast<MutationType>().ToList();

            while (options.Count < count)
            {
                var radiation = allRadiations[Random.Range(0, allRadiations.Count)];
                var option = CreateOptionForRadiation(radiation);

                if (option != null)
                {
                    options.Add(option);
                }
            }

            return options;
        }

        private MutationOption CreateOptionForRadiation(MutationType radiation)
        {
            var validSystems = new List<(SystemType system, SlotType slot, bool isUpgrade, int level)>();

            foreach (var system in manager.Systems)
            {
                var majorSlot = system.MajorSlot;
                var minorSlot = system.MinorSlot;

                if (majorSlot.IsEmpty)
                {
                    validSystems.Add((system.SystemType, SlotType.Major, false, 1));
                }
                else if (majorSlot.RadiationType == radiation && majorSlot.UpgradeLevel < 4)
                {
                    validSystems.Add((system.SystemType, SlotType.Major, true, majorSlot.UpgradeLevel + 1));
                }
                else if (minorSlot.IsEmpty)
                {
                    validSystems.Add((system.SystemType, SlotType.Minor, false, 1));
                }
                else if (minorSlot.RadiationType == radiation && minorSlot.UpgradeLevel < 4)
                {
                    validSystems.Add((system.SystemType, SlotType.Minor, true, minorSlot.UpgradeLevel + 1));
                }
            }

            if (validSystems.Count == 0)
                return null;

            var selected = validSystems[Random.Range(0, validSystems.Count)];
            var effect = factory.CreateEffect(radiation, selected.system, selected.slot);

            if (effect == null)
                return null;

            string description = selected.isUpgrade
                ? $"[UPGRADE] {effect.GetDescriptionAtLevel(selected.level)}"
                : effect.GetDescriptionAtLevel(selected.level);

            return new MutationOption(
                radiation,
                selected.system,
                selected.slot,
                description,
                effect.Icon,
                selected.isUpgrade,
                selected.level
            );
        }

        public void ApplySelection(MutationOption option)
        {
            manager.AssignRadiation(option.radiationType, option.targetSystem);
        }
    }
}