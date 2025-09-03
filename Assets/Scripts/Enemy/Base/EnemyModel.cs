using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Spawn;
using Player.Stats.Runtime;
using UnityEngine;
//using VContainer;



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
    [SerializeField] private GameObject jumpingCoinsTextPrefab;
    [SerializeField] private float heightCoinsTextSpawn = 2f;
    [SerializeField] private float heightTextSpawn = 2f;

    //private FloatingDamageText floatingTextPrefab;
    //private JumpingCoinsText jumpingCoinsTextPrefab;

    //[Inject]
    //public void Construct(FloatingDamageText floatingTextPrefab, JumpingCoinsText jumpingCoinsTextPrefab)
    //{
    //    this.floatingTextPrefab = floatingTextPrefab;
    //    this.jumpingCoinsTextPrefab = jumpingCoinsTextPrefab;

    //    Debug.Log("Floating prefab inyectado: " + (floatingTextPrefab != null));
    //    Debug.Log("Coins prefab inyectado: " + (jumpingCoinsTextPrefab != null));
    //}

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

        view.PlayDamageEffect();
        CurrentHealth -= damageAmount;
        OnHealthChanged?.Invoke(CurrentHealth);

        UpdateHealthBar();

        //Mostrar texto flotante Da�o
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn;
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(damageAmount);

            //var textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            //textObj.Initialize(damageAmount);
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

            //Mostrar texto flotante Coins
            if (jumpingCoinsTextPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * heightCoinsTextSpawn;
                GameObject textObj = Instantiate(jumpingCoinsTextPrefab, spawnPos, Quaternion.identity);
                textObj.GetComponent<JumpingCoinsText>().Initialize(statsSO.CoinsToDrop);

                //var textObj = Instantiate(jumpingCoinsTextPrefab, spawnPos, Quaternion.identity);
                //textObj.Initialize(statsSO.CoinsToDrop);
            }

        }
        OnDeath?.Invoke(this);
    }

    private void UpdateHealthBar()
    {
        if (healthFill == null) return;

        float normalizedHealth = Mathf.Clamp01(CurrentHealth / MaxHealth);
        healthFill.localScale = new Vector3(normalizedHealth, 1f, 1f);

        // Mover la barra hacia la izquierda para que "se vac�e" desde ah�
        healthFill.localPosition = new Vector3((normalizedHealth - 1f) / 2f, 0f, 0f);
    }

}

