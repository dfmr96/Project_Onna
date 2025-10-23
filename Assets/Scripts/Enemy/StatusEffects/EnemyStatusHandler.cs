using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusHandler : MonoBehaviour, IStatusAffectable
{
     [Header("Status Particles")]
    [SerializeField] private ParticleSystem dotBurnEffectPrefab;
    [SerializeField] private ParticleSystem slowedEffectPrefab;
    [SerializeField] private ParticleSystem markedEffectPrefab;

    [Header("Settings Particles")]
    [SerializeField] private Transform spawnPositionEffect;
    [SerializeField] private float yOffsetPositionEffect = 1f;
    [SerializeField] private float offsetBetweenEffects = 0.5f;
  

    private ParticleSystem dotBurnEffectInstance;
    private ParticleSystem slowedEffectInstance;
    private ParticleSystem markedEffectInstance;

    private EnemyModel _enemyModel;
    private BossModel _enemyBossModel;

    private IDamageable _damageable;
    private List<StatusEffect> _activeEffects = new List<StatusEffect>();

    //lista para manejar las part�culas activas
    private readonly List<ParticleSystem> _activeParticles = new List<ParticleSystem>();


    private void Awake()
    {
        _enemyModel = GetComponent<EnemyModel>();
        _enemyBossModel = GetComponent<BossModel>();
        _damageable = GetComponent<IDamageable>();


    }

    private void OnEnable()
    {
        if (_enemyModel != null)
            _enemyModel.OnDeath += HandleEnemyDeath;

        if (_enemyBossModel != null)
            _enemyBossModel.OnDeath += HandleEnemyBossDeath;
    }

    private void OnDisable()
    {
        if (_enemyModel != null)
            _enemyModel.OnDeath -= HandleEnemyDeath;

        if (_enemyBossModel != null)
            _enemyBossModel.OnDeath -= HandleEnemyBossDeath;
    }

    private void HandleEnemyDeath(EnemyModel deadEnemy)
    {
        if (deadEnemy == _enemyModel)
        {
            foreach (var ps in _activeParticles)
            {
                if (ps != null)
                    Destroy(ps.gameObject);
            }

            _activeParticles.Clear();
        }
    }

    private void HandleEnemyBossDeath(BossModel deadEnemy)
    {
        if (deadEnemy == _enemyBossModel)
        {
            foreach (var ps in _activeParticles)
            {
                if (ps != null)
                    Destroy(ps.gameObject);
            }

            _activeParticles.Clear();
        }
    }

    private void Start()
    {
        dotBurnEffectInstance = Instantiate(dotBurnEffectPrefab, spawnPositionEffect);
        dotBurnEffectInstance.Stop();

        slowedEffectInstance = Instantiate(slowedEffectPrefab, spawnPositionEffect);
        slowedEffectInstance.Stop();

        markedEffectInstance = Instantiate(markedEffectPrefab, spawnPositionEffect);
        markedEffectInstance.Stop();
    }

    private void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            if (_activeEffects[i].Update(Time.deltaTime))
            {
                switch (_activeEffects[i])
                {
                    case BurnEffect:
                        StopAndRemoveParticle(dotBurnEffectInstance);
                        break;
                    case SlowEffect:
                        StopAndRemoveParticle(slowedEffectInstance);
                        break;
                    case MarkedEffect:
                        StopAndRemoveParticle(markedEffectInstance);
                        break;
                }

                _activeEffects.RemoveAt(i);
            }
        }
    }

    private void StopAndRemoveParticle(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Stop();
            _activeParticles.Remove(ps);
            RearrangeActiveParticles();
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
                _activeEffects[i] = newEffect;
                return;
            }
        }

        _activeEffects.Add(newEffect);

        ParticleSystem psToActivate = null;

        switch (newEffect)
        {
            case BurnEffect:
                psToActivate = dotBurnEffectInstance;
                break;
            case SlowEffect:
                psToActivate = slowedEffectInstance;
                break;
            case MarkedEffect:
                psToActivate = markedEffectInstance;
                break;
        }

        if (psToActivate != null)
        {
            if (!_activeParticles.Contains(psToActivate))
                _activeParticles.Add(psToActivate);

            psToActivate.transform.localPosition = Vector3.up * yOffsetPositionEffect;
            psToActivate.Clear();
            psToActivate.Play();

            RearrangeActiveParticles();
        }
    }

    private void RearrangeActiveParticles()
    {
        for (int i = 0; i < _activeParticles.Count; i++)
        {
            float offsetX = (i - (_activeParticles.Count - 1) / 2f) * offsetBetweenEffects;
            Vector3 basePos = Vector3.up * yOffsetPositionEffect;
            _activeParticles[i].transform.localPosition = basePos + Vector3.right * offsetX;
        }
    }

    public bool HasStatusEffect<T>() where T : StatusEffect
    {
        foreach (var effect in _activeEffects)
        {
            if (effect is T) return true;
        }
        return false;
    }

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
                totalBonusOrbs += markedEffect.OrbsQuantityAddition;
        }

        return totalBonusOrbs;
    }

    /// <summary>
    /// Returns the damage multiplier from all active WeakenEffects.
    /// </summary>
    public float GetDamageMultiplier()
    {
        float multiplier = 1f;

        foreach (var effect in _activeEffects)
        {
            if (effect is WeakenEffect weakenEffect)
            {
                multiplier *= weakenEffect.DamageMultiplier;
            }
        }

        return multiplier;
    }

    private void OnDestroy()
    {
        foreach (var ps in _activeParticles)
        {
            if (ps != null)
                Destroy(ps.gameObject);
        }

        _activeParticles.Clear();
    }
}
