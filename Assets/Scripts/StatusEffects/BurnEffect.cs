using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : StatusEffect
{
    private float _damagePerTick;
    public string Source { get; private set; } // "Major" o "Minor"

    public BurnEffect(float duration, float damagePerTick, string source = "Default") : base(duration, 1f)
    {
        _damagePerTick = damagePerTick;
        Source = source;
    }

    protected override void OnTick()
    {
        _damageable?.TakeDamage(_damagePerTick);
        Debug.Log($"[BurnEffect] Aplicado daño de quemadura: {_damagePerTick}");
    }

    public override bool IsSameType(StatusEffect other)
    {
        // Solo se considera igual si es BurnEffect y viene de la misma fuente
        if (other is BurnEffect burn)
            return burn.Source == this.Source;

        return false;
    }
}

