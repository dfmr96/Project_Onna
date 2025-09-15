using UnityEngine;

namespace Mutations.Core
{
    public abstract class RadiationEffect : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] protected MutationType radiationType;
        [SerializeField] protected SystemType systemType;
        [SerializeField] protected SlotType slotType;

        [Header("Effect Data")]
        [SerializeField] protected string effectName;
        [TextArea(3, 5)]
        [SerializeField] protected string description;
        [SerializeField] protected Sprite icon;

        [Header("Upgrade")]
        [SerializeField] protected float baseValue;
        [SerializeField] protected float upgradeMultiplier = 1.2f;
        [SerializeField] protected int maxLevel = 4;

        public MutationType RadiationType => radiationType;
        public SystemType SystemType => systemType;
        public SlotType SlotType => slotType;
        public string EffectName => effectName;
        public string Description => description;
        public Sprite Icon => icon;
        public float BaseValue => baseValue;
        public int MaxLevel => maxLevel;

        public float GetValueAtLevel(int level)
        {
            return baseValue * Mathf.Pow(upgradeMultiplier, level - 1);
        }

        public abstract void ApplyEffect(GameObject player, int level = 1);
        public abstract void RemoveEffect(GameObject player);

        public virtual string GetDescriptionAtLevel(int level)
        {
            return description.Replace("{value}", GetValueAtLevel(level).ToString("F1"));
        }
    }
}