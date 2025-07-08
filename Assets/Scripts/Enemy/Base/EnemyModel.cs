using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Spawn;
using Player.Stats.Runtime;
using UnityEngine;


public class EnemyModel : MonoBehaviour, IDamageable
{
    [Header("Stats Config")]
    public EnemyStatsSO statsSO;

    public event Action<float> OnHealthChanged;
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    public event Action<EnemyModel> OnDeath;

    private EnemyView view;
    private EnemyController enemy;
    private OrbSpawner orbSpawner;

    [Header("Floating Damage Text Effect")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float heightTextSpawn = 2f;

    [Header("Health bar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private float heightBarSpawn = 2.5f;
    private Transform healthBar;
    private Transform healthFill;

    private void Start()
    {
        MaxHealth = statsSO.MaxHealth;
        CurrentHealth = MaxHealth;

        view = GetComponent<EnemyView>();
        enemy = GetComponent<EnemyController>();
        orbSpawner = GameManager.Instance.orbSpawner;

        //Instanciar la barra de vida
        if (healthBarPrefab != null)
        {
            GameObject barInstance = Instantiate(healthBarPrefab, transform);
            barInstance.transform.localPosition = new Vector3(0, heightBarSpawn, 0);
            healthBar = barInstance.transform;
            healthFill = healthBar.Find("Fill");
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (enemy.GetShield()) return;

        //Debug.Log("Damagen received: " + damageAmount);
        if (statsSO.RastroOrbOnHit && orbSpawner != null)
        {
            for (int i = 0; i < statsSO.numberOfOrbsOnHit; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }

        CurrentHealth -= damageAmount;
        OnHealthChanged?.Invoke(CurrentHealth);
        //view.PlayDamageAnimation();

        UpdateHealthBar();

        // Mostrar texto flotante
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn; 
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(damageAmount);
        }

        if (CurrentHealth <= 0) Die();
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

        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }


        if (RunData.CurrentCurrency != null)
        {
            RunData.CurrentCurrency.AddCoins(statsSO.CoinsToDrop);

        }
        OnDeath?.Invoke(this);
    }

    private void UpdateHealthBar()
    {
        if (healthFill == null) return;

        float normalizedHealth = Mathf.Clamp01(CurrentHealth / MaxHealth);
        healthFill.localScale = new Vector3(normalizedHealth, 1f, 1f);

        // Mover la barra hacia la izquierda para que "se vacíe" desde ahí
        healthFill.localPosition = new Vector3((normalizedHealth - 1f) / 2f, 0f, 0f);
    }

}

