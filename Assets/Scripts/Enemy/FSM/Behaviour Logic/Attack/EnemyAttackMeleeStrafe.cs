using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Attack-Melee Strafe", menuName = "Enemy Logic/Attack Logic/Melee Attack Strafe")]
public class EnemyAttackMeleeStrafe : EnemyAttackSOBase
{
    //Distintos estados de Strafe
    private enum StrafeState { None, Manual, NavMesh }
    private StrafeState _strafeState = StrafeState.None;

    private bool _isAttacking;
    private int _currentAttackCount;
    private float _strafeTimer;
    private Vector3 _strafeTarget;
    private int _maxConsecutiveAttacks;

    [Header("Strafe Settings")]
    [SerializeField] private int _maxNumberConsecutiveAttacks;
    [SerializeField] private float _strafeCooldown = 1.5f;
    [SerializeField] private float strafeDistance = 4f;
    [SerializeField] private float _strafeSpeed = 10f;
    [SerializeField] private float _strafeStopDistance = 0.1f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _isAttacking = false;

        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        _navMeshAgent.stoppingDistance = 1f;
        _navMeshAgent.updateRotation = true;

        _maxConsecutiveAttacks = Random.Range(1, _maxNumberConsecutiveAttacks);
        _currentAttackCount = 0;
        _strafeState = StrafeState.None;
        _strafeTimer = 0f;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();

        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _navMeshAgent.stoppingDistance = 0f;

        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer > _distanceToCountExit && _strafeState != StrafeState.Manual)
        {
            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

        _strafeTimer += Time.deltaTime;

        if (_strafeState == StrafeState.None)
        {
            if (!_isAttacking)
            {
                if (_timer >= _initialAttackDelay)
                {
                    AttemptAttack();
                    _isAttacking = true;
                    _timer = 0f;
                }
            }
            else if (_timer >= _timeBetweenAttacks && _currentAttackCount < _maxConsecutiveAttacks)
            {
                AttemptAttack();
                _currentAttackCount++;
                _timer = 0f;
            }

            if (_currentAttackCount >= _maxConsecutiveAttacks && _strafeTimer >= _strafeCooldown)
            {
                _strafeState = StrafeState.Manual;
                TryStrafe();
            }
        }
        else if (_strafeState == StrafeState.Manual)
        {
            Vector3 direction = (_strafeTarget - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _strafeTarget);

            if (distance > _strafeStopDistance)
            {
                Vector3 movement = direction * _strafeSpeed * Time.deltaTime;
                transform.position += movement;
                enemy.Rb.MovePosition(enemy.Rb.position + movement);
            }
            else
            {
                EndStrafe();
            }
        }
        else if (_strafeState == StrafeState.NavMesh)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                EndStrafe();
            }
        }
    }

    private void TryStrafe()
    {
        _enemyView.PlayStrafeAnimation();

        Vector3[] directions = {
            transform.right,
            -transform.right,
            -transform.forward
        };

        directions = directions.OrderBy(x => Random.value).ToArray();

        NavMeshHit hit;
        foreach (Vector3 dir in directions)
        {
            Vector3 targetPos = transform.position + dir * strafeDistance;

            if (NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
            {
                _strafeTarget = hit.position;
                _strafeState = StrafeState.Manual;

                _navMeshAgent.isStopped = true;
                _navMeshAgent.ResetPath();
                _navMeshAgent.velocity = Vector3.zero;

                return;
            }
        }

        _strafeState = StrafeState.None;
    }

    private void EndStrafe()
    {
        _strafeState = StrafeState.None;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = _enemyModel.statsSO.moveSpeed;
        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;

        _currentAttackCount = 0;
        _maxConsecutiveAttacks = Random.Range(1, _maxNumberConsecutiveAttacks);
        _strafeTimer = 0f;
    }

    private void AttemptAttack()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= _distanceToCountExit)
        {
            _enemyView.Animator.applyRootMotion = true;
            _enemyView.PlayAttackAnimation(true);
            TriggerAttackColorEffect();
        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
        _isAttacking = false;
        _strafeState = StrafeState.None;

        _enemyView.PlayAttackAnimation(false);
        _enemyView.Animator.applyRootMotion = false;
    }

    public override void Initialize(GameObject gameObject, EnemyController enemy)
    {
        base.Initialize(gameObject, enemy);
    }
}
