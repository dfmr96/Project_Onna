using Player;
using System.Collections;
using UnityEngine;

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

    //private float _distanceToCountExit = 3f;



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

    }

    //ActionEvent de Ataque
    //public void AnimationAttackFunc()
    //{

    //    if (_playerTransform != null)
    //    {
    //        float distanceToPlayer = Vector3.Distance(_playerTransform.position, transform.position);

    //        //doble comprobacion por si se aleja
    //        if (distanceToPlayer <= _distanceToCountExit)
    //        {
    //            IDamageable damageablePlayer = _playerTransform.GetComponent<IDamageable>();
    //            _bossController.ExecuteAttack(damageablePlayer);

    //        }
    //    }
    //}

    private Coroutine _shootCoroutine;

    [SerializeField] private int burstCount = 3;           // Cantidad de disparos por ráfaga
    [SerializeField] private float burstInterval = 0.2f;   // Tiempo entre disparos
    //[SerializeField] private float spreadAngle = 5f;
    [SerializeField] private int pelletsPerShot = 5;          // cuántos proyectiles por escopetazo
    [SerializeField] private float spreadAngle = 15f;         // dispersión en grados

    public void AnimationShootProjectileFunc()
    {
        //Debug.Log("Entro animacion");
        //if (projectileSpawner == null) return;


        //Transform firePoint = _bossController.firePoint;

        //Vector3 targetPos = _playerTransform.position;
        //targetPos.y = firePoint.position.y;

        //Vector3 dir = (targetPos - firePoint.position).normalized;

        //projectileSpawner.SpawnProjectile(firePoint.position, dir, _bossModel.statsSO.ShootForce, _bossModel.statsSO.AttackDamage);

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

    private void ShootSingleProjectile()
    {
        if (projectileSpawner == null || _playerTransform == null) return;

        Transform firePoint = _bossController.firePoint;

        Vector3 targetPos = _playerTransform.position;
        targetPos.y = firePoint.position.y;

        //Vector3 dir = (targetPos - firePoint.position).normalized;

        Vector3 dir = (targetPos - firePoint.position).normalized;
        dir = Quaternion.Euler(0, Random.Range(-spreadAngle, spreadAngle), 0) * dir;

        projectileSpawner.SpawnProjectile(firePoint.position, dir, _bossModel.statsSO.ShootForce, _bossModel.statsSO.ProjectileDamage);
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
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Vector3 spreadDir = Quaternion.Euler(0, angle, 0) * baseDir;

            projectileSpawner.SpawnProjectile(
                firePoint.position,
                spreadDir,
                _bossModel.statsSO.ShootForce,
                _bossModel.statsSO.ProjectileDamage
            );
        }
    }



    //Animaciones
    public void PlayAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
    }

    public void PlayProjectilesAttackAnimation()
    {
        animator.SetTrigger("IsProjectilesAttacking");
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
        animator.SetTrigger("Idle");
    }

    public void PlayMovingAnimation(float moveSpeed)
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
    }

    public void PlayStunnedAnimation()
    {
        animator.SetTrigger("IsStunned");
    }

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

    public void ShootShotgun() { audioSource.PlayOneShot(shootAudioClip); }


    public void PlayDamageAnimation()
    {
        animator.SetTrigger("IsDamaged");
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger("IsDead");
     


    }

    public void UpdateHealthBar(float healthPercentage)
    {
        //health bar logic
    }
}

