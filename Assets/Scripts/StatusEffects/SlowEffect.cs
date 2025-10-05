using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowEffect : StatusEffect
{
    private float _slowAmount; // porcentaje de reducción (0.5 = 50%)
    private float _originalSpeed;
    private NavMeshAgent _agent;
    public string Source { get; private set; }

    public SlowEffect(float duration, float slowAmount, string source = null)
        : base(duration)
    {
        // Nunca permitir un slow del 100% (congela al enemigo)
        _slowAmount = Mathf.Clamp(slowAmount, 0f, 0.9f);
        Source = source;
    }

    public override void Initialize(IDamageable damageable)
    {
        base.Initialize(damageable);

        // Buscar el NavMeshAgent en el objeto del enemigo
        if (damageable is Component comp)
        {
            _agent = comp.GetComponent<NavMeshAgent>();
            if (_agent != null)
            {
         
                _originalSpeed = _agent.speed;
                _agent.speed = _originalSpeed * (1f - _slowAmount);
                Debug.Log($"[SlowEffect] Aplicado: {_agent.gameObject.name} ahora va a {_agent.speed:F2} (antes {_originalSpeed:F2})");
            }
        }
    }

    protected override void OnTick()
    {
        // no aplica
    }

    public override bool Update(float deltaTime)
    {
        bool expired = base.Update(deltaTime);
        if (expired)
            OnExpire();
        return expired;
    }

    private void OnExpire()
    {
        if (_agent != null)
        {
            _agent.speed = _originalSpeed; // restaurar velocidad
            Debug.Log($"[SlowEffect] Expiró: {_agent.gameObject.name} restaurado a {_originalSpeed:F2}");
        }
    }

    public override bool IsSameType(StatusEffect other)
    {
        // Solo se considera igual si es BurnEffect y viene de la misma fuente
        if (other is SlowEffect slow)
            return slow.Source == this.Source;

        return false;
    }
}

