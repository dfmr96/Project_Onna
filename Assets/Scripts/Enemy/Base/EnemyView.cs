using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EnemyView : MonoBehaviour
{
    private Animator animator;
    private Transform _playerTransform;
    private EnemyController _enemyController;
    private EnemyModel _enemyModel;
    private ProjectileSpawner projectileSpawner;

    private float _distanceToCountExit = 3f;

    private Renderer targetRenderer;
    private Color flashColor = Color.white;
    private float flashDuration = .3f;
    private Material material;
    private Color originalColor;

    public event Action OnAttackStarted;
    public event Action OnAttackImpact;

    //for enemy torret
    private bool useFirstFirePoint = true;
    //Cambiar mas adelante
    public Transform firePoint2;
    public Transform turretHead;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private float recoilAngle = 5f;
    private float recoilDistance = 2f;
    private float recoilSpeed = 5f;
    [SerializeField] private ParticleSystem deathParticlesPrefab;
    [SerializeField] private Material[] damageMaterials;
    [SerializeField] private Material[] deathMaterials;
    private Material[] originalMaterials;// material temporal de daño
    [SerializeField] private int[] materialIndexesToFlash = { 0 };


    private float deadAngle = 40f;
    private AudioSource audioSource;

    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip damagedAudioClip;
    private bool isDead = false;

    private Coroutine flashCoroutine = null;
    private Coroutine recoilCoroutine;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();


    }
    public Animator Animator => animator;

    private void Start()
    {
        _playerTransform = PlayerHelper.GetPlayer().transform;
        projectileSpawner = GameManager.Instance.projectileSpawner;
        _enemyController = GetComponent<EnemyController>();
        _enemyModel = GetComponent<EnemyModel>();
        targetRenderer = GetComponentInChildren<Renderer>();

        material = targetRenderer.material;
        //originalColor = material.GetColor("_Color");
        originalColor = material.color;

        originalMaterials = targetRenderer.materials;
        //torret
        if (turretHead != null)
        {
            initialRotation = turretHead.localRotation;
            initialPosition = turretHead.localPosition;
        }



    }


    public void AnimationAttackStarted()
    {
        OnAttackStarted?.Invoke();
    }

    //ActionEvent de Ataque
    public void AnimationAttackFunc()
    {
        OnAttackImpact?.Invoke();

        if (_playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);

            //doble comprobacion por si se aleja
            if (distanceToPlayer <= _distanceToCountExit)
            {
                IDamageable damageablePlayer = _playerTransform.GetComponent<IDamageable>();
                _enemyController.ExecuteAttack(damageablePlayer);

            }
        }
    }

    //Evento para ataques tipo charge
    public void AnimationChargeAttackFunc()
    {
        OnAttackImpact?.Invoke();

        if (_playerTransform != null)
        {
            float hitRadius = _enemyModel.statsSO.AttackRange;
            Vector3 hitCenter = transform.position + transform.forward * 1f; //Fix para asegurar que le pegue 

            float distanceToPlayer = Vector3.Distance(_playerTransform.position, hitCenter);

            if (distanceToPlayer <= hitRadius)
            {
                IDamageable damageablePlayer = _playerTransform.GetComponent<IDamageable>();
                if (damageablePlayer != null)
                    _enemyController.ExecuteAttack(damageablePlayer);
            }
        }
    }




    public void AnimationShootProjectileFunc()
    {
        if (projectileSpawner == null) return;

        Transform firePoint = _enemyController.firePoint;

        Vector3 targetPos = _playerTransform.position;
        targetPos.y = firePoint.position.y;
        Vector3 dir = (targetPos - firePoint.position).normalized;

        projectileSpawner.SpawnProjectile(firePoint.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);

        audioSource.PlayOneShot(shootAudioClip);
    }

    public void TorretShootProjectileFunc()
    {
        if (projectileSpawner == null) return;

        Transform firePoint = _enemyController.firePoint;

        Vector3 targetPos;
        Vector3 dir;

        if (useFirstFirePoint)
        {
            targetPos = _playerTransform.position;
            targetPos.y = firePoint.position.y;
            dir = (targetPos - firePoint.position).normalized;

            projectileSpawner.SpawnProjectile(firePoint.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);
            DoRecoil();

        }
        else
        {
            targetPos = _playerTransform.position;
            targetPos.y = firePoint2.position.y;
            dir = (targetPos - firePoint2.position).normalized;

            projectileSpawner.SpawnProjectile(firePoint2.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.currentDamage, _enemyModel);
            DoRecoil();

        }

        //alterna entre firepoints
        useFirstFirePoint = !useFirstFirePoint;
    }

    public void DoRecoil()
    {
        if (turretHead == null) return;

        //StopAllCoroutines(); // Detener recoil anteriores

        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);

        StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        // Calculamos posici�n y rotaci�n de retroceso
        Quaternion recoilRotation = initialRotation * Quaternion.Euler(-recoilAngle, 0f, 0f);
        Vector3 recoilPosition = initialPosition - turretHead.localRotation * Vector3.forward * recoilDistance;

        // Aplicamos el retroceso instant�neo
        turretHead.localRotation = recoilRotation;
        turretHead.localPosition = recoilPosition;

        // Lerp suave hacia la posici�n y rotaci�n original
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            turretHead.localRotation = Quaternion.Slerp(recoilRotation, initialRotation, t);
            turretHead.localPosition = Vector3.Lerp(recoilPosition, initialPosition, t);
            yield return null;
        }

        // Aseguramos que termine en el lugar exacto
        turretHead.localRotation = initialRotation;
        turretHead.localPosition = initialPosition;
    }

    public void PlayDeathParticles()
    {
        if (deathParticlesPrefab != null)
        {
            ParticleSystem deathParticlesInstance = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            deathParticlesInstance.Play();

            if (turretHead != null)
                turretHead.localRotation = Quaternion.Euler(deadAngle, 0f, 0f);

            Destroy(deathParticlesInstance.gameObject, 2f);
        }
    }














    //Animaciones
    public void PlayAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void PlayMeleeAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsMeleeAttacking", isAttacking);
    }

    public void PlayStrafeAnimation()
    {
        animator.SetTrigger("IsStrafing");
    }

    public bool GetBoolAttackAnimation()
    {
        return animator.GetBool("IsAttacking");
    }

    public void PlayIdleAnimation()
    {
        //animator.SetTrigger("Idle");
    }

    public void PlayMovingAnimation(float moveSpeed)
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
    }

    public void PlayStunnedAnimation()
    {
        animator.SetTrigger("IsStunned");
    }

    public void HandleDamage()
    {
        animator.SetTrigger("IsDamaged");
        audioSource?.PlayOneShot(damagedAudioClip);
    }

    //public void PlayDamageAnimation()
    //{
    //    animator.SetTrigger("IsDamaged");
    //}

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("IsDead");
        isDead = true;

        // activamos los materiales de muerte permanentemente
        targetRenderer.materials = GetFittedMaterials(deathMaterials);

        // cancelamos cualquier flash en curso
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        PlayDeathParticles();
    }

    private void ActivateDamageMaterials()
    {
        Material[] newMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (i < damageMaterials.Length)
                newMaterials[i] = damageMaterials[i];
            else
                newMaterials[i] = damageMaterials[damageMaterials.Length - 1];
        }

        targetRenderer.materials = newMaterials;
    }


    public void UpdateHealthBar(float healthPercentage)
    {
        //health bar logic
    }

    public void PlayDamageEffect()
    {
        if (isDead) return; // si está muerto no hacemos flash

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashDamageMaterialsCoroutine());
    }

    private IEnumerator FlashDamageMaterialsCoroutine()
    {
        // ⚡ aplicamos materiales de daño temporalmente
        targetRenderer.materials = GetFittedMaterials(damageMaterials);

        yield return new WaitForSeconds(flashDuration);

        // 🌀 restauramos materiales originales solo si sigue vivo
        if (!isDead)
            targetRenderer.materials = originalMaterials;

        flashCoroutine = null;
    }

    private IEnumerator FlashCoroutine()
    {
        //material.SetColor("_Color", flashColor);

        //yield return new WaitForSeconds(flashDuration);

        //material.SetColor("_Color", originalColor);

        //material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        // material.color = originalColor;
        //flashCoroutine = null;
    }
    
    private Material[] GetFittedMaterials(Material[] source)
    {
        Material[] newMaterials = new Material[originalMaterials.Length];

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (i < source.Length)
                newMaterials[i] = source[i];
            else
                newMaterials[i] = source[source.Length - 1]; // repetir el último si faltan
        }

        return newMaterials;
    }
}

