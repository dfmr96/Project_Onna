using UnityEngine;
using UnityEngine.AI;

public class MarkedEffect : StatusEffect
{
    private float _damage;
    public string Source { get; private set; }

    public MarkedEffect(float duration, float damage, string source = null)
        : base(duration)
    {
        _damage = damage;
        Source = source;
    }

    public override void Initialize(IDamageable damageable) 
    {
        base.Initialize(damageable);
        _damageable.TakeDamage(_damage);
    }

    public override bool IsSameType(StatusEffect other)
    {
        if (other is MarkedEffect marked)
            return marked.Source == this.Source;

        return false;
    }

    protected override void OnTick() { }
}
