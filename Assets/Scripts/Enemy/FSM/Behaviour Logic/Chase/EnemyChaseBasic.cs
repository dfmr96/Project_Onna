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
        _navMeshAgent.speed = _enemyModel.currentSpeed * _speedAgentMultiply;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed * _speedAgentMultiply;

        //Soluciona efecto de hielo en desaceleracion
        _navMeshAgent.acceleration = 999f;      // Respuesta rápida a cambios
        _navMeshAgent.angularSpeed = 1000f;     // Lo máximo posible, que no limite
        _navMeshAgent.stoppingDistance = 0;     // No frena por cercanía
        _navMeshAgent.autoBraking = false;      // Que no frene automáticamente
        _navMeshAgent.updateRotation = false;   // Vamos a rotar manualmente
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _navMeshAgent.speed = _enemyModel.currentSpeed;
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


        Vector3 direction = (playerTransform.position - transform.position);
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _enemyModel.statsSO.rotationSpeed * Time.deltaTime * _speedAgentMultiply);
        }
    }

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}

