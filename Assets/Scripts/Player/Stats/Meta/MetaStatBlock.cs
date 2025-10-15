using System;
using System.Collections.Generic;
using Player.Stats.Interfaces;
using UnityEngine;

namespace Player.Stats.Meta
{
    [Serializable]
    public class MetaStatBlock : IStatContainer, IStatSource, IStatTarget
    {
        [SerializeField] private StatContainerLogic container = new();
        private IStatSource baseStatSource;

        public float Get(StatDefinition stat) => container.Get(stat);
        public void Set(StatDefinition stat, float value) => container.Set(stat, value);
        public IReadOnlyDictionary<StatDefinition, float> All => container.All;
        public void Clear() => container.Clear();
        public void RebuildLookup() => container.RebuildLookup();
        
        public void AddFlatBonus(StatDefinition stat, float value)
        {
            float current = Get(stat);
            float debugCurrent = current;
            float newValue = current + value;
            Set(stat, newValue);

            Debug.Log($"[MetaStatBlock] Applying +{value} to '{stat.name} | Current={debugCurrent} | Added={value} | NewValue={newValue}");
        }

        public void AddPercentBonus(StatDefinition stat, float percent)
        {
            if (baseStatSource == null)
            {
                Debug.LogWarning($"[MetaStatBlock] baseStatSource is null. Cannot apply percent bonus to '{stat.name}'");
                return;
            }

            float metaValue = Get(stat);
            float baseValue = baseStatSource.Get(stat);
            float debugBase = baseValue;
            float delta = baseValue * (percent / 100f);
            float newMeta = metaValue + delta;

            Set(stat, newMeta);

            Debug.Log($"[MetaStatBlock] Applying +{percent}% to '{stat.name} | Current ={debugBase} | Bonus={delta}, NewMeta={newMeta}");
        }

        public void AddMultiplierBonus(StatDefinition stat, float factor)
        {
            float current = Get(stat);
            float debugCurrent = current;
            float newValue = current * factor;
            Set(stat, newValue);

            Debug.Log($"[MetaStatBlock] Applying multiplier x{factor} to '{stat.name} | Current={debugCurrent} | NewValue={newValue}");
        }

        
        public void InjectBaseSource(IStatSource source)
        {
            baseStatSource = source;
        }
    }
}