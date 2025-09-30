using UnityEngine;

public abstract class StatusEffect
{
    public float Duration { get; private set; }
    public float TickInterval { get; private set; }
    public float Elapsed { get; private set; }

    protected IDamageable _damageable;

    public StatusEffect(float duration, float tickInterval = 1f)
    {
        Duration = duration;
        TickInterval = tickInterval;
        Elapsed = 0f;
    }

    public void Initialize(IDamageable damageable)
    {
        _damageable = damageable;
        Debug.Log($"[StatusEffect] Inicializado con {damageable}");

    }

    public bool Update(float deltaTime)
    {
        Elapsed += deltaTime;

        if (Elapsed >= TickInterval)
        {
            OnTick();
            Elapsed = 0f;
        }

        Duration -= deltaTime;
        return Duration <= 0;
    }

    protected abstract void OnTick();

    public virtual bool IsSameType(StatusEffect other)
    {
        return this.GetType() == other.GetType();
    }
}

