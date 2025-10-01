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

    // ======================
    // 🎨 EFECTOS VISUALES
    // ======================
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem deathParticlesPrefab;   // NUEVO
    [SerializeField] private ParticleSystem damageParticlesPrefab;  // NUEVO
    [SerializeField] private Material[] damageMaterials;            // NUEVO
    [SerializeField] private Material[] deathMaterials;             // NUEVO

    private Renderer targetRenderer;                                // NUEVO
    private Material[] originalMaterials;                           // NUEVO
    private Coroutine flashCoroutine = null;                        // NUEVO
    private float flashDuration = .3f;                              // NUEVO
    private bool isDead = false;                                    // NUEVO

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

        // Guardamos renderer y materiales originales
        targetRenderer = GetComponentInChildren<Renderer>();
        if (targetRenderer != null)
            originalMaterials = targetRenderer.materials;
    }

    // ===========================================================
    // 📌 EFECTOS VISUALES
    // ===========================================================

    public void HandleDamage()   // similar a EnemyView
    {
        animator.SetTrigger("IsDamaged");

        // Partículas de daño
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

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("IsDead");
        isDead = true;

        // Aplicamos materiales de muerte
        if (targetRenderer != null && deathMaterials.Length > 0)
            targetRenderer.materials = GetFittedMaterials(deathMaterials);

        // Cancelamos cualquier flash en curso
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

    public void PlayDamageEffect()
    {
        if (isDead || targetRenderer == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashDamageMaterialsCoroutine());
    }

    private IEnumerator FlashDamageMaterialsCoroutine()
    {
        targetRenderer.materials = GetFittedMaterials(damageMaterials);

        yield return new WaitForSeconds(flashDuration);

        if (!isDead)
            targetRenderer.materials = originalMaterials;

        flashCoroutine = null;
    }

    private Material[] GetFittedMaterials(Material[] source)
    {
        Material[] newMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (i < source.Length)
                newMaterials[i] = source[i];
            else
                newMaterials[i] = source[source.Length - 1];
        }

        return newMaterials;
    }

    // ===========================================================
    // 🔫 DISPAROS (como ya tenías)
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
    // 🎬 ANIMACIONES
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

    // ===========================================================
    // ❤️ VIDA
    // ===========================================================
    public void UpdateHealthBar(float healthPercentage)
    {
        // lógica UI barra de vida
    }
}
