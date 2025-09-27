using System.Collections.Generic;
using UnityEngine;

namespace Mutations.Core
{
    [CreateAssetMenu(fileName = "New Radiation Effect Factory", menuName = "Mutations/Radiation Effect Factory")]
    public class RadiationEffectFactory : ScriptableObject
    {
        [System.Serializable]
        public class EffectEntry
        {
            public MutationType radiationType;
            public SystemType systemType;
            public SlotType slotType;
            public RadiationEffect effect;
        }

        [SerializeField] private List<EffectEntry> effectDatabase;

        public RadiationEffect CreateEffect(MutationType radiation, SystemType system, SlotType slot)
        {
            var entry = effectDatabase.Find(e =>
                e.radiationType == radiation &&
                e.systemType == system &&
                e.slotType == slot);

            return entry?.effect;
        }

        public List<RadiationEffect> GetEffectsForSystem(SystemType system)
        {
            var effects = new List<RadiationEffect>();

            foreach (var entry in effectDatabase)
            {
                if (entry.systemType == system && entry.effect != null)
                {
                    effects.Add(entry.effect);
                }
            }

            return effects;
        }

        public List<RadiationEffect> GetEffectsForRadiation(MutationType radiation)
        {
            var effects = new List<RadiationEffect>();

            foreach (var entry in effectDatabase)
            {
                if (entry.radiationType == radiation && entry.effect != null)
                {
                    effects.Add(entry.effect);
                }
            }

            return effects;
        }

        #if UNITY_EDITOR
        [NaughtyAttributes.Button("Validate Database")]
        private void ValidateDatabase()
        {
            int missingCount = 0;
            var systems = System.Enum.GetValues(typeof(SystemType));
            var radiations = System.Enum.GetValues(typeof(MutationType));
            var slots = System.Enum.GetValues(typeof(SlotType));

            foreach (SystemType system in systems)
            {
                foreach (MutationType radiation in radiations)
                {
                    foreach (SlotType slot in slots)
                    {
                        var exists = effectDatabase.Exists(e =>
                            e.systemType == system &&
                            e.radiationType == radiation &&
                            e.slotType == slot &&
                            e.effect != null);

                        if (!exists)
                        {
                            Debug.LogWarning($"Missing effect: {radiation} + {system} + {slot}");
                            missingCount++;
                        }
                    }
                }
            }

            Debug.Log($"Database validation complete. Missing effects: {missingCount}");
        }
        #endif
    }
}