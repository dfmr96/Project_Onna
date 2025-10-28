using UnityEngine;

/// <summary>
/// Status effect that reduces the damage an enemy deals (outgoing damage).
/// Used by Cherenkov Minor mutation to weaken enemy attacks.
/// </summary>
public class DamageReductionEffect : StatusEffect
{
    public float OutgoingDamageMultiplier { get; private set; }
    public string Source { get; private set; }

    /// <summary>
    /// Creates a damage reduction effect.
    /// </summary>
    /// <param name="duration">How long the effect lasts</param>
    /// <param name="damageMultiplier">Multiplier for outgoing damage (e.g., 0.7 = enemy deals 70% damage)</param>
    /// <param name="source">Identifier for the source of this effect</param>
    public DamageReductionEffect(float duration, float damageMultiplier, string source = "Default")
        : base(duration, 1f)
    {
        OutgoingDamageMultiplier = damageMultiplier;
        Source = source;
    }

    protected override void OnTick()
    {
        // No damage applied, this effect only reduces enemy outgoing damage
    }

    public override bool IsSameType(StatusEffect other)
    {
        if (other is DamageReductionEffect damageReduction)
        {
            return damageReduction.Source == this.Source;
        }
        return false;
    }
}
