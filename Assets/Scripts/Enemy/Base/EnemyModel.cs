using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Spawn;
using Player.Stats.Runtime;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyModel : MonoBehaviour, IDamageable
{
    [Header("Stats Config")]
    public EnemyStatsSO statsSO;
    public EnemyVariantSO variantSO;

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

    [Header("Health bar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private float heightBarSpawn = 2.5f;
    private Transform healthBar;
    private Transform healthFill;
    private Animator healthBarAnimator;
    private Vector3 originalBarLocalPos;
    private Coroutine shakeRoutine;


    [Header("Buff Stats Info:")]
    public float currentHealth;
    public float currentDamage;
    public float currentSpeed;
    public float currentAttackTimeRate;


    private float healthMultiplier = 1f;
    private float damageMultiplier = 1f;
    private float speedMultiplier = 1f;
    private float attackTimeRateMultiplier = 1f;
    private float damageReductionDuringAttackMultiplier = 1f;
    private float orbsMultiplier = 0f;

    private void Start()
    {


        ////////////   Buffs/Debuffs   //////////////
        if (variantSO != null)
        {
            healthMultiplier = variantSO.healthMultiplier;
            damageMultiplier = variantSO.damageMultiplier;
            speedMultiplier = variantSO.speedMultiplier;
            attackTimeRateMultiplier = variantSO.attackChargeTimeMultiplier;
            damageReductionDuringAttackMultiplier = variantSO.damageReductionDuringAttack;
            orbsMultiplier = variantSO.extraOrbsOnDeath;
        }


        //MaxHealth = statsSO.MaxHealth;
        //Vida
        MaxHealth = statsSO.MaxHealth * healthMultiplier;
        CurrentHealth = MaxHealth;
        currentHealth = CurrentHealth;

        //Velocidad Movimiento
        currentSpeed = statsSO.moveSpeed * speedMultiplier;

        //Poder de Ataque
        currentDamage = statsSO.AttackDamage * damageMultiplier;

        //Tiempo entre ataques
        currentAttackTimeRate = statsSO.AttackTimeRate * attackTimeRateMultiplier;


        view = GetComponent<EnemyView>();
        enemy = GetComponent<EnemyController>();
        orbSpawner = GameManager.Instance.orbSpawner;

        //Instanciar la barra de vida
        if (healthBarPrefab != null)
        {
            GameObject barInstance = Instantiate(healthBarPrefab, transform);
            barInstance.transform.localPosition = new Vector3(0, heightBarSpawn, 0.05f);

            healthBar = barInstance.transform;
            healthFill = healthBar.Find("Fill");

            healthBarAnimator = barInstance.GetComponentInChildren<Animator>();
        }

        if (healthBar != null)
            originalBarLocalPos = healthBar.localPosition;
            
        //if (variantSO != null && variantSO.variantType == EnemyVariantType.Yellow)
        //{
        //    Debug.Log("Soy un enemigo amarillo fortificado");
        //}

        //if (variantSO != null)
        //{
        //    switch (variantSO.variantType)
        //    {
        //        case EnemyVariantType.Green:
        //            // Aplica DoT al jugador
        //            //player.ApplyDot(variantSO.dotDamage, variantSO.dotDuration);
        //            break;

        //        case EnemyVariantType.Purple:
        //            //if (CurrentHealth <= 0 && !hasExploded)
        //            //{
        //            //    Explode(variantSO.explosionRadius, variantSO.explosionDamage);
        //            //    hasExploded = true;
        //            //}
        //            break;

        //        case EnemyVariantType.Dark:
        //            if (enemy.IsChargingAttack())
        //            {
        //                damageAmount *= (1f - variantSO.damageReductionDuringAttack);
        //            }
        //            break;
        //    }
        //}
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

        //Aplica Defensa si hay Buff (actualmente solo variante Dark)
        if ((statsSO.currentState == "EnemyAttackState") && variantSO.hasDefensivePhase)
        {
            CurrentHealth -= damageAmount * damageReductionDuringAttackMultiplier;
        }
        else
        {
            CurrentHealth -= damageAmount;
        }


        OnHealthChanged?.Invoke(CurrentHealth);

        UpdateHealthBar();

        //Mostrar texto flotante Da�o
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
            for (int i = 0; i < statsSO.numberOfOrbsOnDeath + orbsMultiplier; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }

        if (healthBar != null)
        {
            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
                shakeRoutine = null;
            }
            Destroy(healthBar.gameObject);
        }

        if (RunData.CurrentCurrency != null)
        {
            RunData.CurrentCurrency.AddCoins(statsSO.CoinsToDrop);

            if (jumpingCoinsTextPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * heightCoinsTextSpawn;
                GameObject textObj = Instantiate(jumpingCoinsTextPrefab, spawnPos, Quaternion.identity);
                textObj.GetComponent<JumpingCoinsText>().Initialize(statsSO.CoinsToDrop);
            }
        }

        OnDeath?.Invoke(this);
    }


    private void UpdateHealthBar()
    {
        float normalizedHealth = Mathf.Clamp01(CurrentHealth / MaxHealth);

        if (healthFill != null)
        {
            healthFill.localScale = new Vector3(normalizedHealth, 2f, 1f);
            healthFill.localPosition = new Vector3((normalizedHealth - 1f) / 2f, 0f, 0f);
        }
        if (healthBarAnimator != null)
        {
            healthBarAnimator.SetFloat("Health", normalizedHealth);
        }
        
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
            shakeRoutine = StartCoroutine(ShakeBar(0.15f, 0.1f));
    }

    private IEnumerator ShakeBar(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (healthBar == null) yield break;

            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            healthBar.localPosition = originalBarLocalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (healthBar != null)
            healthBar.localPosition = originalBarLocalPos;
    }


    public void ApplyDebuffDoT(float dotDuration, float dps)
    {
        throw new NotImplementedException();
    }

}

