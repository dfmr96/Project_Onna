using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : StatusEffect
{
    private float _damagePerTick;
    public string Source { get; private set; } // "Major" o "Minor"

    private ParticleSystem _particlePrefab;



    public BurnEffect(float duration, float damagePerTick, string source = "Default", ParticleSystem particlePrefab = null) : base(duration, 1f)
    {
        _damagePerTick = damagePerTick;
        Source = source;
        _particlePrefab = particlePrefab;
    }

    protected override void OnTick()
    {
        _damageable?.TakeDamage(_damagePerTick);

        if (_particlePrefab != null)
        {
            ParticleSystem ps = Object.Instantiate(_particlePrefab, _damageable.Transform, Quaternion.identity);
            ps.Play();
            Object.Destroy(ps.gameObject, ps.main.duration);
        }

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

