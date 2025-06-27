using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pillar : MonoBehaviour, IDamageable
{
 
    public event Action<Pillar> OnPillarDestroyed;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    private bool isDestroyed = false;

    [Header("Floating Damage Text Effect")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float heightTextSpawn = 2f;
    [SerializeField] private ParticleSystem particleExplosion;
    private ParticleSystem explosionInstance;



    private OrbSpawner orbSpawner;
    private BossModel _bossModel;


 

    private void Start()
    {
       

        orbSpawner = GameManager.Instance.orbSpawner;
        _bossModel = GetComponentInParent<BossModel>();

        MaxHealth = _bossModel.statsSO.PillarMaxHealth;
        CurrentHealth = MaxHealth;

        //Instanciar una vez la explosion y detenerla
        if (particleExplosion != null)
        {
            explosionInstance = Instantiate(particleExplosion, transform.position, Quaternion.identity);
            explosionInstance.Stop();
        }

        ResetPillar();

    }

    void SpawnParticle(Vector3 pos)
    {
        if (explosionInstance != null)
        {
            explosionInstance.transform.position = pos;
            explosionInstance.Play();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDestroyed) return;

        CurrentHealth -= damageAmount;

        // Mostrar texto flotante
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn;
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(damageAmount);
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        //Efecto de destruccion
        SpawnParticle(transform.position);

        gameObject.SetActive(false); 

        OnPillarDestroyed?.Invoke(this);

        if (_bossModel.statsSO.PillarRastroOrbOnDeath && orbSpawner != null)
        {
            for (int i = 0; i < _bossModel.statsSO.pillarNumberOfOrbsOnDeath; i++)
            {
                orbSpawner.SpawnHealingOrb(transform.position, transform.forward);
            }
        }
    }

   
    public void ResetPillar()
    {
        CurrentHealth = MaxHealth;
        isDestroyed = false;
        gameObject.SetActive(true);
    }
}

