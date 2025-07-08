using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Escape-Escape from Player Distance", menuName = "Enemy Logic/Escape Logic/Escape from Player Distance")]

public class EnemyEscapeFromPlayerArea : EnemyEscapeSOBase
{
    [SerializeField] private int _speedAgentMultiply;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        //_navMeshAgent.speed = _enemyModel.statsSO.moveSpeed * _speedAgentMultiply;
        //_navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed * _speedAgentMultiply; 

    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();

        ResetValues();


    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < escapeDistance)
        {
            Vector3 directionAway = (transform.position - playerTransform.position).normalized;
            Vector3 targetPos = transform.position + directionAway * desiredDistance;

            _navMeshAgent.SetDestination(targetPos);
        }
        else
        {
            enemy.fsm.ChangeState(enemy.ChaseState); 
        }

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
