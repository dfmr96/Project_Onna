using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Chase-Chase Basic", menuName = "Enemy Logic/Chase Logic/Chase Basic")]

public class EnemyChaseBasic : EnemyChaseSOBase
{
    [SerializeField] private int _speedAgentMultiply;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _navMeshAgent.speed = _enemyModel.statsSO.moveSpeed * _speedAgentMultiply;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed * _speedAgentMultiply; 
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _navMeshAgent.speed = _enemyModel.statsSO.moveSpeed;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed;

    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        _navMeshAgent.SetDestination(playerTransform.position);

        if (distanceToPlayer > _enemyModel.statsSO.AttackRange)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(playerTransform.position);
        }
        else
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.ResetPath(); 
            enemy.fsm.ChangeState(enemy.AttackState);
        }

        if (!enemy.isAggroed)
        {
            enemy.fsm.ChangeState(enemy.SearchState);
        }

        //if (enemy.isWhitinCombatRadius)
        //{
        //    enemy.fsm.ChangeState(enemy.AttackState);
        //}
    }

    public override void Initialize(GameObject gameObject, EnemyController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}

