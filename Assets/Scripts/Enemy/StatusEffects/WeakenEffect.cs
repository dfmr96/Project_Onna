using UnityEngine;

public class WeakenEffect : StatusEffect
{
    public float DamageMultiplier { get; private set; } // 1.25 = +25% daño recibido
    public string Source { get; private set; }

    public WeakenEffect(float duration, float damageMultiplier, string source = "Default") : base(duration, 1f)
    {
        DamageMultiplier = damageMultiplier;
        Source = source;
    }

    protected override void OnTick()
    {
        // WeakenEffect no hace daño por sí mismo, solo marca al enemigo
        // El multiplicador se consulta cuando el enemigo recibe daño
        Debug.Log($"[WeakenEffect] Enemy weakened: {DamageMultiplier:P0} damage multiplier");
    }

    public override bool IsSameType(StatusEffect other)
    {
        // Solo se considera igual si es WeakenEffect y viene de la misma fuente
        if (other is WeakenEffect weaken)
            return weaken.Source == this.Source;

        return false;
    }
}