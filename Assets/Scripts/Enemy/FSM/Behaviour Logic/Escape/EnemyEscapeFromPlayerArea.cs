using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Escape-Escape from Player Distance", menuName = "Enemy Logic/Escape Logic/Escape from Player Distance")]

public class EnemyEscapeFromPlayerArea : EnemyEscapeSOBase
{
    [SerializeField] private int _speedAgentMultiply;
    private float _speedBase;
    private float _angularSpeedBase;
    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _speedBase = _navMeshAgent.speed;
        _angularSpeedBase = _navMeshAgent.angularSpeed;

        _navMeshAgent.speed = _speedBase * _speedAgentMultiply;
        _navMeshAgent.angularSpeed = _angularSpeedBase * _speedAgentMultiply; 

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
            ResetValues();
            enemy.fsm.ChangeState(enemy.ChaseState); 
        }

    }

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();

        _navMeshAgent.speed = _speedBase;
        _navMeshAgent.angularSpeed = _angularSpeedBase;
    }

}
