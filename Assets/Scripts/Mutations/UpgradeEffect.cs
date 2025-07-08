using NaughtyAttributes;
using Player.Stats;
using Player.Stats.Interfaces;
using Player.Stats.Meta;
using UnityEngine;

namespace Mutations
{
    public abstract class UpgradeEffect : ScriptableObject
    {
        [SerializeField] protected StatReferences statRefs;
        public abstract void Apply(IStatTarget player, float value, ValueMode mode);
    }
}