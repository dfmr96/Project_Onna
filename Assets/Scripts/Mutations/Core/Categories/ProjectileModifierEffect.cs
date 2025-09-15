using Mutations.Core;
using UnityEngine;

namespace Mutations.Core.Categories
{
    public enum ProjectileModifierType
    {
        Penetration,
        Damage,
        FireRate,
        SlowEffect,
        MarkingEffect
    }

    public abstract class ProjectileModifierEffect : RadiationEffect
    {
        [Header("Projectile Modifier Settings")]
        [SerializeField] protected ProjectileModifierType modifierType;
        [SerializeField] protected float modifierMultiplier = 1.2f;
        [SerializeField] protected int maxTargets = -1;
        [SerializeField] protected bool affectsSecondaryEffects = false;

        public ProjectileModifierType ModifierType => modifierType;
        public float ModifierMultiplier => modifierMultiplier;
        public int MaxTargets => maxTargets;
        public bool AffectsSecondaryEffects => affectsSecondaryEffects;

        protected float GetModifierValueAtLevel(int level)
        {
            return baseValue * Mathf.Pow(modifierMultiplier, level - 1);
        }

        protected int GetMaxTargetsAtLevel(int level)
        {
            if (maxTargets <= 0) return -1;
            return maxTargets + (level - 1);
        }

        public override string GetDescriptionAtLevel(int level)
        {
            return modifierType switch
            {
                ProjectileModifierType.Penetration => $"Disparos atraviesan {GetMaxTargetsAtLevel(level)} enemigos",
                ProjectileModifierType.Damage => $"Aumenta daño de disparos en {GetModifierValueAtLevel(level):F1}x",
                ProjectileModifierType.FireRate => $"Modifica cadencia en {GetModifierValueAtLevel(level):F1}x",
                ProjectileModifierType.SlowEffect => $"Disparos ralentizan enemigos {GetModifierValueAtLevel(level):F1}s",
                ProjectileModifierType.MarkingEffect => $"Disparos marcan enemigos (+{GetModifierValueAtLevel(level):F1}% daño recibido)",
                _ => "Modifica proyectiles"
            };
        }

        public override void ApplyEffect(GameObject player, int level = 1)
        {
            var weaponController = player.GetComponent<Player.Weapon.WeaponController>();
            if (weaponController != null)
            {
                RegisterProjectileModifier(weaponController, level);
            }
        }

        public override void RemoveEffect(GameObject player)
        {
            var weaponController = player.GetComponent<Player.Weapon.WeaponController>();
            if (weaponController != null)
            {
                UnregisterProjectileModifier(weaponController);
            }
        }

        protected abstract void RegisterProjectileModifier(Player.Weapon.WeaponController controller, int level);
        protected abstract void UnregisterProjectileModifier(Player.Weapon.WeaponController controller);
    }
}