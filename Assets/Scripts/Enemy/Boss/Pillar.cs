using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pillar : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event Action<Pillar> OnPillarDestroyed;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private bool isDestroyed = false;

    [Header("Floating Damage Text Effect")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float heightTextSpawn = 2f;

    private void Awake()
    {
        ResetPillar();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDestroyed) return;

        currentHealth -= damageAmount;

        // Mostrar texto flotante
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn;
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(damageAmount);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        //Efecto de destruccion
        gameObject.SetActive(false); 

        OnPillarDestroyed?.Invoke(this);
    }

    public void ResetPillar()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
        gameObject.SetActive(true);
    }
}

