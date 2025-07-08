using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.PlayerSettings;

public class Pillar : MonoBehaviour, IDamageable
{

    [Header("Movement")]
    [SerializeField] private float riseHeight = 2f;
    [SerializeField] private float targetHeight = 1.5f;
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private float shakeIntensity = 0.1f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public event Action<Pillar> OnPillarDestroyed;

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    private bool isDestroyed = false;

    [Header("Floating Damage Text Effect")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float heightTextSpawn = 2f;
    [SerializeField] private ParticleSystem particleExplosion;

    private ParticleSystem explosionInstance;
    private AudioSource audioSource;

    private OrbSpawner orbSpawner;
    [SerializeField] private BossModel _bossModel;

    [Header("Audio")]
    [SerializeField] private AudioClip spawnClip;
    [SerializeField] private AudioClip destroyClip;



    //UI
    public event Action<float, float> OnPillarHealthChanged;

    private void Start()
    {
        orbSpawner = GameManager.Instance.orbSpawner;
        //_bossModel = GetComponentInParent<BossModel>();

        MaxHealth = _bossModel.statsSO.PillarMaxHealth;
        CurrentHealth = MaxHealth;

        //Instanciar una vez la explosion y detenerla
        if (particleExplosion != null)
        {
            explosionInstance = Instantiate(particleExplosion, transform.position, Quaternion.identity);
            explosionInstance.Stop();
        }
 

        CurrentHealth = MaxHealth;
        isDestroyed = false;
        gameObject.SetActive(true);

        targetPosition = transform.position;

        audioSource = GetComponent<AudioSource>();
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

        //UI
        OnPillarHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
            audioSource?.PlayOneShot(destroyClip);
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

   
    public void ResetPillar(Transform targetTransform)
    {
        CurrentHealth = MaxHealth;
        isDestroyed = false;
        gameObject.SetActive(true);

        //pos inicial hundida
        targetPosition = new Vector3(targetTransform.position.x, targetHeight, targetTransform.position.z);

        initialPosition = targetPosition - Vector3.up * riseHeight;
        transform.position = initialPosition;

        //UI
        OnPillarHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        //Sonido
        audioSource?.PlayOneShot(spawnClip);

        //empezamos a subir
        StartCoroutine(RiseUpCoroutine());
    }

    private IEnumerator RiseUpCoroutine()
    {
        float elapsed = 0f;
        float duration = riseHeight / riseSpeed;



        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Movimiento de subida
            Vector3 basePos = Vector3.Lerp(initialPosition, targetPosition, t);

            // Agrego temblor aleatorio en X y Z (opcional: podés hacerlo solo en X o solo en Y si querés)
            float shakeX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            float shakeZ = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            Vector3 shakeOffset = new Vector3(shakeX, 0, shakeZ);

            transform.position = basePos + shakeOffset;

            yield return null;
        }

        transform.position = targetPosition;
    }
}

