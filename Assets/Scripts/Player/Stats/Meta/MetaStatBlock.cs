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

        public Dictionary<string, float> ToSerializableDict()
        {
            var dict = new Dictionary<string, float>();
            foreach (var entry in container.GetRawList())
                if (entry.stat != null)
                    dict[entry.stat.name] = entry.value;
            return dict;
        }

        public void LoadFromSerializableDict(Dictionary<string, float> data, StatRegistry registry)
        {
            if (registry == null)
            {
                Debug.LogError("❌ StatRegistry es null al intentar cargar MetaStats.");
                return;
            }

            container.Clear();

            foreach (var pair in data)
            {
                var def = registry.GetByName(pair.Key);
                if (def != null)
                {
                    container.Set(def, pair.Value);
                }
                else
                {
                    Debug.LogWarning($"⚠️ StatDefinition no encontrado en el registry: '{pair.Key}'");
                }
            }

            container.RebuildLookup();
        }


        public void AddFlatBonus(StatDefinition stat, float value)
        {
            float current = Get(stat);
            Set(stat, current + value);
        }

        public void AddPercentBonus(StatDefinition stat, float percent)
        {
            float metaValue = Get(stat);
            float baseValue = baseStatSource?.Get(stat) ?? 0f;
            float basePlusMeta = baseValue + metaValue;
            float delta = basePlusMeta * (percent / 100f);
            Set(stat, metaValue + delta);
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