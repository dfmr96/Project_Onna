using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Charging attack Tank type", menuName = "Enemy Logic/Attack Logic/Charging attack Tank type")]
public class EnemyAttackCharge : EnemyAttackSOBase
{
    private bool _hasAttackedOnce = false;

    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 1f;

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

        if (!_hasAttackedOnce && !isCharging && _timer >= _initialAttackDelay)
        {
            StartCharge();
            return;
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);


            _navMeshAgent.Move(chargeDirection * chargeSpeed * Time.deltaTime);

            //if (chargeTimer >= chargeDuration)
            //{
            //    EndCharge(); 
            //}

            if (distanceToPlayer < 1 || chargeTimer >= chargeDuration)
            {
                EndCharge();
            }
        }


    }


    public override void Initialize(GameObject gameObject, EnemyController enemy)
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


   


    private void StartCharge()
    {
        if (playerTransform == null) return;

        isLookingPlayer = false;

        _enemyView.PlayAttackAnimation(true);

        //enemy.SetShield(false);

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
        enemy.SetShield(false);
        isLookingPlayer = true;


        enemy.fsm.ChangeState(enemy.IdleState);
    }



}