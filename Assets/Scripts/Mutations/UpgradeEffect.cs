using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Meta;
using UnityEngine;

namespace Mutations
{
    public abstract class UpgradeEffect : ScriptableObject
    {
        [Header("🔬 Test Config (Editor Only)")]
        [SerializeField] protected StatReferences statRefs;
        [SerializeField] protected MetaStatBlock testMetaStats;
        [SerializeField] protected StatBlock testBaseStats;
        public abstract void Apply(IStatTarget player);
    }
}