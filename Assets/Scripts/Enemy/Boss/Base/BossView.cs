using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossView : MonoBehaviour
{
    private Animator animator;
    private Transform _playerTransform;
    private BossController _bossController;
    private BossModel _bossModel;
    private ProjectileSpawner projectileSpawner;

    //private float _distanceToCountExit = 3f;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public Animator Animator => animator;

    private void Start()
    {
        _playerTransform = PlayerHelper.GetPlayer().transform;
        projectileSpawner = GameManager.Instance.projectileSpawner;
        _bossController = GetComponent<BossController>();
        _bossModel = GetComponent<BossModel>();

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

    //public void AnimationShootProjectileFunc()
    //{
    //    if (projectileSpawner == null) return;

    //    Transform firePoint = _bossController.firePoint;

    //    Vector3 targetPos = _playerTransform.position;
    //    targetPos.y = firePoint.position.y; 

    //    Vector3 dir = (targetPos - firePoint.position).normalized;

    //    projectileSpawner.SpawnProjectile(firePoint.position, dir, _bossModel.statsSO.ShootForce, _bossModel.statsSO.AttackDamage);
        

    //}



    //Animaciones
    public void PlayAttackAnimation(bool isAttacking)
    {
        animator.SetBool("IsAttacking", isAttacking);
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

