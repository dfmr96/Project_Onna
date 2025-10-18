using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusHandler : MonoBehaviour, IStatusAffectable
{
    private List<StatusEffect> _activeEffects = new List<StatusEffect>();
    private IDamageable _damageable;

    private void Awake()
    {
        _damageable = GetComponent<IDamageable>();
    }


    private void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            if (_activeEffects[i].Update(Time.deltaTime))
                _activeEffects.RemoveAt(i);
        }
    }

    public void ApplyStatusEffect(StatusEffect newEffect)
    {
        if (_damageable == null)
            _damageable = GetComponent<IDamageable>();

        newEffect.Initialize(_damageable);

        for (int i = 0; i < _activeEffects.Count; i++)
        {
            if (_activeEffects[i].IsSameType(newEffect))
            {
                _activeEffects[i] = newEffect; // reinicia contador
                return;
            }
        }

        _activeEffects.Add(newEffect);
    }

    public bool HasStatusEffect<T>() where T : StatusEffect
    {
        foreach (var effect in _activeEffects)
        {
            if (effect is T) return true;
        }
        return false;
    }


    // Método helper para HasStatusEffect con fuente específica
    public bool HasStatusEffect<T>(string source = null) where T : StatusEffect
    {
        foreach (var effect in _activeEffects)
        {
            if (effect is BurnEffect burn)
            {
                if (source != null && burn.Source != source)
                    continue;

                if (typeof(T) == typeof(BurnEffect))
                    return true;
            }
        }
        return false;
    }

    public int GetBonusOrbsFromMark()
    {
        int totalBonusOrbs = 0;

        foreach (var effect in _activeEffects)
        {

            if (effect is MarkedOrbsEffect markedEffect)
            {
                totalBonusOrbs += markedEffect.OrbsQuantityAddition;
            }
        }
        return totalBonusOrbs;
    }
}
