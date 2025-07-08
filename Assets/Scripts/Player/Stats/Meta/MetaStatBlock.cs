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
            Set(stat, current + value);
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
            float delta = baseValue * (percent / 100f);
            Set(stat, metaValue + delta);

            Debug.Log($"[MetaStatBlock] Applying +{percent}% to '{stat.name}': Base={baseValue}, Bonus={delta}, NewMeta={metaValue + delta}");
        }

        public void AddMultiplierBonus(StatDefinition stat, float factor)
        {
            float current = Get(stat);
            Set(stat, current * factor);
        }
        
        public void InjectBaseSource(IStatSource source)
        {
            baseStatSource = source;
        }
    }
}