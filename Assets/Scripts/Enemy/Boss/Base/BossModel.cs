using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Spawn;
using Player.Stats.Runtime;
using UnityEngine;


public class BossModel : MonoBehaviour, IDamageable
{
    [Header("Stats Config")]
    public BossStatsSO statsSO;

    public event Action<float> OnHealthChanged;
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public Vector3 Transform => transform.position;
    private FloatingTextSpawner floatingTextSpawner;
    private JumpingTextSpawner _jumpingTextSpawner;


    public event Action<BossModel> OnDeath;


    private BossView view;
    private BossController enemy;
    private OrbSpawner orbSpawner;

    [Header("Floating Damage Text Effect")]
    [SerializeField] private GameObject jumpingCoinsTextPrefab;
    [SerializeField] private float heightCoinsTextSpawn = 2f;


    private void Start()
    {
        MaxHealth = statsSO.MaxHealth;
        CurrentHealth = MaxHealth;
 
        view = GetComponent<BossView>();
        enemy = GetComponent<BossController>();
        orbSpawner = GameManager.Instance.orbSpawner;


        OnHealthChanged?.Invoke(CurrentHealth);
        floatingTextSpawner = EnemyManager.Instance.floatingTextSpawner;
        _jumpingTextSpawner = EnemyManager.Instance.jumpingTextSpawner;

    }

    public void PrintMessage(String text, float lifeTime)
    {
  
        if (EnemyManager.Instance != null && EnemyManager.Instance.floatingTextSpawner != null)
        {
            floatingTextSpawner.SpawnFloatingText(transform.position, text, lifeTime);
        }
    }
    public void TakeDamage(float damageAmount)
    {
        //if (enemy.GetShield()) return;


        if (statsSO.RastroOrbOnHit && orbSpawner != null)
        {
            for (int i = 0; i < statsSO.numberOfOrbsOnHit; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }

        float newHealth = CurrentHealth - damageAmount;
        CurrentHealth = GetCappedHealth(newHealth);

        OnHealthChanged?.Invoke(CurrentHealth);



 
        if (EnemyManager.Instance != null && EnemyManager.Instance.floatingTextSpawner != null)
        {
            floatingTextSpawner.SpawnFloatingText(transform.position, damageAmount);
        }

        if (CurrentHealth <= 0) Die();
    }


    private float GetCappedHealth(float newHealth)
    {
        float healthPercent = CurrentHealth / MaxHealth;

        if (healthPercent > 0.66f)
            return Mathf.Max(newHealth, MaxHealth * 0.66f); 
        else if (healthPercent > 0.33f)
            return Mathf.Max(newHealth, MaxHealth * 0.33f);
        else
            return newHealth; 
    }




    public void Die()
    {
        if (statsSO.RastroOrbOnDeath && orbSpawner != null)
        {
            for (int i = 0; i < statsSO.numberOfOrbsOnDeath; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }



        if (RunData.CurrentCurrency != null)
        {
            RunData.CurrentCurrency.AddCoins(statsSO.CoinsToDrop);

            //Mostrar texto flotante Coins
                 if (EnemyManager.Instance != null && EnemyManager.Instance.jumpingTextSpawner != null)
            {
                _jumpingTextSpawner.SpawnFloatingText(transform.position, statsSO.CoinsToDrop);
            }

        }
        OnDeath?.Invoke(this);
    }

    public void ApplyDebuffDoT(float dotDuration, float dps)
    {
        Debug.LogWarning("ApplyDebuffDoT called, but not implemented yet.");
    }
}

