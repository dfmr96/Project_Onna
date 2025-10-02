using Player;
using System.Collections;
using UnityEngine;
using System;

public class BossView : MonoBehaviour
{
    private Animator animator;
    private Transform _playerTransform;
    private BossController _bossController;
    private BossModel _bossModel;
    private ProjectileSpawner projectileSpawner;
    private AudioSource audioSource;

    [Header("Audio")]
    [SerializeField] private AudioClip laserShootAudioClip;
    [SerializeField] private AudioClip shootAudioClip;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem deathParticlesPrefab;
    [SerializeField] private ParticleSystem damageParticlesPrefab;
    [SerializeField] private Material[] damageMaterials;
    [SerializeField] private Material[] deathMaterials;

    private Renderer[] targetRenderers;         // 🔹 Todos los renderers del boss
    private Material[][] originalMaterials;     // 🔹 Materiales originales de cada renderer
    private Coroutine flashCoroutine = null;
    private float flashDuration = .3f;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public Animator Animator => animator;

    private void Start()
    {
        _playerTransform = PlayerHelper.GetPlayer().transform;
        _bossController = GetComponent<BossController>();
        _bossModel = GetComponent<BossModel>();
        projectileSpawner = GameManager.Instance.projectileSpawner;
        audioSource = GetComponent<AudioSource>();

        // 🔹 Guardamos TODOS los renderers y sus materiales originales
        targetRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[targetRenderers.Length][];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            originalMaterials[i] = targetRenderers[i].materials;
        }
    }

    // ===========================================================
    // 💥 EFECTOS DE DAÑO
    // ===========================================================
    public void HandleDamage()
    {
        animator.SetTrigger("IsDamaged");

        if (damageParticlesPrefab != null)
        {
            ParticleSystem damageParticlesInstance = Instantiate(
                damageParticlesPrefab,
                transform.position + Vector3.up * 1f,
                Quaternion.identity,
                transform
            );

            damageParticlesInstance.Play();
            Destroy(damageParticlesInstance.gameObject, 2f);
        }

        PlayDamageEffect();
    }

    public void PlayDamageEffect()
    {
        if (isDead || targetRenderers == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashDamageMaterialsCoroutine());
    }

    private IEnumerator FlashDamageMaterialsCoroutine()
    {
        // 🔹 Aplicamos el material de daño a todos los renderers
        foreach (Renderer rend in targetRenderers)
        {
            Material[] glitchedMaterials = new Material[rend.materials.Length];
            for (int i = 0; i < glitchedMaterials.Length; i++)
                glitchedMaterials[i] = damageMaterials[0];

            rend.materials = glitchedMaterials;
        }

        yield return new WaitForSeconds(flashDuration);

        // 🔹 Restauramos materiales originales
        if (!isDead)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                targetRenderers[i].materials = originalMaterials[i];
            }
        }

        flashCoroutine = null;
    }

    // ===========================================================
    // ☠️ EFECTOS DE MUERTE
    // ===========================================================
    public void PlayDeathAnimation()
    {
        animator.SetTrigger("IsDead");
        isDead = true;

        // 🔹 Aplicamos materiales de muerte en todas las partes
        for (int i = 0; i < targetRenderers.Length; i++)
            targetRenderers[i].materials = GetFittedMaterials(deathMaterials, originalMaterials[i].Length);

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        PlayDeathParticles();
    }

    private void PlayDeathParticles()
    {
        if (deathParticlesPrefab != null)
        {
            ParticleSystem deathParticlesInstance = Instantiate(
                deathParticlesPrefab,
                transform.position + Vector3.up,
                Quaternion.identity
            );

            deathParticlesInstance.Play();
            Destroy(deathParticlesInstance.gameObject, 2f);
        }
    }

    private Material[] GetFittedMaterials(Material[] source, int slots)
    {
        Material[] newMaterials = new Material[slots];
        for (int i = 0; i < slots; i++)
        {
            if (i < source.Length)
                newMaterials[i] = source[i];
            else
                newMaterials[i] = source[source.Length - 1];
        }
        return newMaterials;
    }

    // ===========================================================
    // 🔫 DISPAROS
    // ===========================================================
    private Coroutine _shootCoroutine;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.2f;
    [SerializeField] private int pelletsPerShot = 5;
    [SerializeField] private float spreadAngle = 15f;

    public void AnimationShootProjectileFunc()
    {
        if (_shootCoroutine != null)
            StopCoroutine(_shootCoroutine);

        _shootCoroutine = StartCoroutine(ShootBurstCoroutine());
    }

    private IEnumerator ShootBurstCoroutine()
    {
        for (int i = 0; i < burstCount; i++)
        {
            ShootShotgunProjectile();
            yield return new WaitForSeconds(burstInterval);
        }
    }

    private void ShootShotgunProjectile()
    {
        if (projectileSpawner == null || _playerTransform == null) return;

        Transform firePoint = _bossController.firePoint;
        Vector3 targetPos = _playerTransform.position;
        targetPos.y = firePoint.position.y;

        Vector3 baseDir = (targetPos - firePoint.position).normalized;

        for (int i = 0; i < pelletsPerShot; i++)
        {
            float angle = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
            Vector3 spreadDir = Quaternion.Euler(0, angle, 0) * baseDir;

            projectileSpawner.SpawnProjectileBoss(
                firePoint.position,
                spreadDir,
                _bossModel.statsSO.ShootForce,
                _bossModel.statsSO.ProjectileDamage
            );
        }
    }

    // ===========================================================
    // 🎭 ANIMACIONES
    // ===========================================================
    public void PlayAttackAnimation(bool isAttacking) => animator.SetBool("IsAttacking", isAttacking);
    public void PlayProjectilesAttackAnimation() => animator.SetTrigger("IsProjectilesAttacking");
    public void PlayStrafeAnimation() => animator.SetTrigger("IsStrafing");
    public bool GetBoolAttackAnimation() => animator.GetBool("IsAttacking");
    public void PlayIdleAnimation() => animator.SetTrigger("Idle");
    public void PlayMovingAnimation(float moveSpeed) => animator.SetFloat("MoveSpeed", moveSpeed);
    public void PlayStunnedAnimation() => animator.SetTrigger("IsStunned");

    // ===========================================================
    // 🔊 SONIDO
    // ===========================================================
    public void StartLaserShoot()
    {
        audioSource.clip = laserShootAudioClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopLaserShoot()
    {
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.Stop();
    }

    public void ShootShotgun() => audioSource.PlayOneShot(shootAudioClip);

    public void UpdateHealthBar(float healthPercentage)
    {
        // lógica UI barra de vida
    }
}
