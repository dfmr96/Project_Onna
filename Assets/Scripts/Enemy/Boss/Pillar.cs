using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pillar : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float undergroundDepth = 6f;
    [SerializeField] private float finalHeight = 0f;
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private float shakeIntensity = 0.1f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public event Action<Pillar> OnPillarDestroyed;

    public Vector3 Transform => transform.position;

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

    [Header("Visual Flash")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;// opcional: si querés que algún GO no cambie de material
    private Coroutine flashCoroutine;

    private List<Renderer> pillarRenderers = new List<Renderer>();
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    //UI
    public event Action<float, float> OnPillarHealthChanged;

    private void Start()
    {
        orbSpawner = GameManager.Instance.orbSpawner;

        MaxHealth = _bossModel.statsSO.PillarMaxHealth;
        CurrentHealth = MaxHealth;

        if (particleExplosion != null)
        {
            explosionInstance = Instantiate(particleExplosion, transform.position, Quaternion.identity);
            explosionInstance.Stop();
        }

        isDestroyed = false;
        gameObject.SetActive(true);

        targetPosition = transform.position;

        audioSource = GetComponent<AudioSource>();

        // Guardamos los materiales originales al inicio
        pillarRenderers.AddRange(GetComponentsInChildren<Renderer>());
        foreach (var rend in pillarRenderers)
        {
            originalMaterials[rend] = rend.materials;
        }

        OnPillarHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        //Fuerza al pilar a empezar hundido
        targetPosition = new Vector3(transform.position.x, finalHeight, transform.position.z);
        initialPosition = targetPosition + Vector3.down * undergroundDepth;
        transform.position = initialPosition;
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

        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightTextSpawn;
            GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.GetComponent<FloatingDamageText>().Initialize(damageAmount);
        }

        OnPillarHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        audioSource?.PlayOneShot(destroyClip);

        // Disparamos glitch/flash visual
        if (flashMaterial != null)
        {
            StartCoroutine(FlashCoroutine());
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashCoroutine()
    {
        flashCoroutine = StartCoroutine(FlashRoutine());
        yield return null;
    }

    private IEnumerator FlashRoutine()
    {
        // cambiar a flash
        foreach (var rend in pillarRenderers)
        {
            var mats = new Material[rend.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = flashMaterial;
            }
            rend.materials = mats;
        }

        yield return new WaitForSeconds(flashDuration);

        // restaurar
        RestoreOriginalMaterials();
    }


    private void RestoreOriginalMaterials()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        foreach (var rend in pillarRenderers)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
                rend.materials = originalMaterials[rend];
        }
    }

    public void Die()
    {
        if (isDestroyed) return;

        isDestroyed = true;
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

        //restaurar materiales para que no quede “pegado” el flash
        RestoreOriginalMaterials();

        //Siempre fuerza target y posición inicial
        targetPosition = new Vector3(transform.position.x, finalHeight, transform.position.z);
        initialPosition = targetPosition + Vector3.down * undergroundDepth;
        transform.position = initialPosition;

        OnPillarHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        audioSource?.PlayOneShot(spawnClip);

        StartCoroutine(RiseUpCoroutine());
    }

    private IEnumerator RiseUpCoroutine()
    {
        float elapsed = 0f;
        float duration = undergroundDepth / riseSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 basePos = Vector3.Lerp(initialPosition, targetPosition, t);

            float shakeX = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            float shakeZ = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            Vector3 shakeOffset = new Vector3(shakeX, 0, shakeZ);

            transform.position = basePos + shakeOffset;

            yield return null;
        }

        transform.position = targetPosition;
    }

    public void ApplyDebuffDoT(float dotDuration, float dps)
    {
        Debug.LogWarning("ApplyDebuffDoT called, but not implemented yet.");
    }
}
