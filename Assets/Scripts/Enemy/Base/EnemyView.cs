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

    private Coroutine flashCoroutine = null;
    private Renderer targetRenderer; 
    private Color flashColor = Color.white;
    private float flashDuration = 0.1f;
    private Material material;
    private Color originalColor;

    public event Action OnAttackStarted;
    public event Action OnAttackImpact;

    //for enemy torret
    private bool useFirstFirePoint = true;
    //Cambiar mas adelante
    private Coroutine recoilCoroutine;

    public Transform firePoint2;
    public Transform turretHead;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private float recoilAngle = 5f;
    private float recoilDistance = 2f;
    private float recoilSpeed = 5f;
    [SerializeField] private ParticleSystem deathParticlesPrefab;

    private float deadAngle = 40f;
    private AudioSource audioSource;

    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip damagedAudioClip;



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
        originalColor = material.color;

        //torret
        initialRotation = turretHead.localRotation;
        initialPosition = turretHead.localPosition;


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

    public void AnimationShootProjectileFunc()
    {
        if (projectileSpawner == null) return;

        Transform firePoint = _enemyController.firePoint;

        Vector3 targetPos = _playerTransform.position;
        targetPos.y = firePoint.position.y; 
        Vector3 dir = (targetPos - firePoint.position).normalized;

        projectileSpawner.SpawnProjectile(firePoint.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.statsSO.AttackDamage);
        
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

            projectileSpawner.SpawnProjectile(firePoint.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.statsSO.AttackDamage);
            DoRecoil();

        }
        else
        {
            targetPos = _playerTransform.position;
            targetPos.y = firePoint2.position.y;
            dir = (targetPos - firePoint2.position).normalized;

            projectileSpawner.SpawnProjectile(firePoint2.position, dir, _enemyModel.statsSO.ShootForce, _enemyModel.statsSO.AttackDamage);
            DoRecoil();

        }

        //alterna entre firepoints
        useFirstFirePoint = !useFirstFirePoint; 
    }

    public void DoRecoil()
    {
        if (turretHead == null) return;

        if (recoilCoroutine != null)
            StopCoroutine(recoilCoroutine);
        
        StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        // Calculamos posición y rotación de retroceso
        Quaternion recoilRotation = initialRotation * Quaternion.Euler(-recoilAngle, 0f, 0f);
        Vector3 recoilPosition = initialPosition - turretHead.localRotation * Vector3.forward * recoilDistance;

        // Aplicamos el retroceso instantáneo
        turretHead.localRotation = recoilRotation;
        turretHead.localPosition = recoilPosition;

        // Lerp suave hacia la posición y rotación original
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
            deathParticlesPrefab.Play();

            turretHead.localRotation = Quaternion.Euler(deadAngle, 0f, 0f);

            Destroy(deathParticlesPrefab, 2f);
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
     


    }

    public void UpdateHealthBar(float healthPercentage)
    {
        //health bar logic
    }

    public void PlayDamageEffect()
    {

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        material.color = originalColor;

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        material.color = originalColor;
        flashCoroutine = null;



    }
}

