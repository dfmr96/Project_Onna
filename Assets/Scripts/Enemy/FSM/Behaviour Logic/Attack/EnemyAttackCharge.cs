using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Charging attack Tank type", menuName = "Enemy Logic/Attack Logic/Charging attack Tank type")]
public class EnemyAttackCharge : EnemyAttackSOBase
{
    private bool _hasAttackedOnce = false;

    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float postChargeDelay = 0.5f;
    private float postChargeTimer;
    private bool isPostCharging;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private Vector3 chargeDirection;



    public override void DoEnterLogic()
    {
        base.DoEnterLogic();


        //enemy.SetShield(false);

        _navMeshAgent.SetDestination(playerTransform.position);
        _hasAttackedOnce = false;
        isCharging = false;

        _navMeshAgent.isStopped = true;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();


        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        _timer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (isPostCharging)
        {
            postChargeTimer += Time.deltaTime;
            if (postChargeTimer >= postChargeDelay)
            {
                isPostCharging = false;
                _timer = 0f;
                enemy.SetShield(true);

            }
            return;
        }

        //Si se aleja del rango de ataque cambiar a busqueda
        if (distanceToPlayer > _distanceToCountExit)
        {
            EndAttackAnimations();
            enemy.SetShield(true);

            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            _navMeshAgent.Move(chargeDirection * chargeSpeed * Time.deltaTime);

            if (distanceToPlayer < 1 || chargeTimer >= chargeDuration)
            {
                EndCharge();
            }
            return;
        }

        //Si ya ataco una vez, espera el tiempo entre ataques
        if (_hasAttackedOnce)
        {
            if (_timer >= _timeBetweenAttacks)
            {
                _hasAttackedOnce = false;
                _timer = 0f;
            }
            return;
        }

        //Si no ataco todavia y cumplio el delay inicial
        if (_timer >= _initialAttackDelay)
        {
            if (distanceToPlayer <= meleeRange)
            {
                StartMeleeAttack();
            }
            else 
            {
                StartCharge();
            }
        }
    }




    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();

        _hasAttackedOnce = false;
        isCharging = false;
        chargeTimer = 0f;
        chargeDirection = Vector3.zero;

        if (_navMeshAgent != null)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.ResetPath();
        }

    }


    private void EndAttackAnimations()
    {
        _enemyView.PlayAttackAnimation(false);
        _enemyView.PlayMeleeAttackAnimation(false);
    }

    private void StartMeleeAttack()
    {
        isCharging = false;
        _enemyView.PlayAttackAnimation(false);

        //enemy.SetShield(false);

        _enemyView.PlayMeleeAttackAnimation(true);
        _hasAttackedOnce = true;
        _timer = 0f;
    }
    private void StartCharge()
    {
        if (playerTransform == null) return;

        isLookingPlayer = false;

        _enemyView.PlayMeleeAttackAnimation(false);

        _enemyView.PlayAttackAnimation(true);

        enemy.SetShield(false);

        _navMeshAgent.isStopped = false;
        _navMeshAgent.ResetPath();

        chargeDirection = (playerTransform.position - enemy.transform.position).normalized;
        chargeTimer = 0f;
        isCharging = true;
        _hasAttackedOnce = true;
        _timer = 0f;
    }

    private void EndCharge()
    {
        isCharging = false;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;

        _enemyView.PlayAttackAnimation(false);
        //enemy.SetShield(true);
        isLookingPlayer = true;

        isPostCharging = true;
        postChargeTimer = 0f;
    }



}