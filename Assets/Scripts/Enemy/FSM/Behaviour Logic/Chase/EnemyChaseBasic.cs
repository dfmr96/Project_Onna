using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Chase-Chase Basic", menuName = "Enemy Logic/Chase Logic/Chase Basic")]

public class EnemyChaseBasic : EnemyChaseSOBase
{
    [SerializeField] private int _speedAgentMultiply;
    [SerializeField] private float flankRadius = 3f;

    private EnemyManager coordinator;
    private Vector3 targetPos;

    private float currentSpeed;
    private float currentAngSpeed;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        currentSpeed = _navMeshAgent.speed;
        currentAngSpeed = _navMeshAgent.angularSpeed;

        _navMeshAgent.speed = _enemyModel.currentSpeed * _speedAgentMultiply;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed * _speedAgentMultiply;

        //Soluciona efecto de hielo en desaceleracion
        _navMeshAgent.acceleration = 999f;      // Respuesta rápida a cambios
        _navMeshAgent.angularSpeed = 1000f;     // Lo máximo posible, que no limite
        _navMeshAgent.stoppingDistance = 0;     // No frena por cercanía
        _navMeshAgent.autoBraking = false;      // Que no frene automáticamente
        _navMeshAgent.updateRotation = false;   // Vamos a rotar manualmente

        coordinator = EnemyManager.Instance;
        coordinator?.RegisterEnemy(transform);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _navMeshAgent.speed = currentSpeed;
        _navMeshAgent.angularSpeed = currentAngSpeed;

        coordinator?.UnregisterEnemy(transform);


    }

    //public override void DoFrameUpdateLogic()
    //{
    //    base.DoFrameUpdateLogic();

    //    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

    //    //_navMeshAgent.SetDestination(playerTransform.position);

    //    if (distanceToPlayer > _enemyModel.statsSO.AttackRange)
    //    {
    //        //_navMeshAgent.isStopped = false;
    //        //_navMeshAgent.SetDestination(playerTransform.position);
    //        // Persecución
    //        if (_navMeshAgent.isStopped)
    //            _navMeshAgent.isStopped = false;

    //        _navMeshAgent.SetDestination(playerTransform.position);
    //    }
    //    else
    //    {

    //        //_navMeshAgent.isStopped = true;
    //        //_navMeshAgent.velocity = Vector3.zero;
    //        //_navMeshAgent.ResetPath(); 

    //        // Listo para atacar frenamos de verdad
    //        if (!_navMeshAgent.isStopped)
    //        {
    //            _navMeshAgent.isStopped = true;
    //            _navMeshAgent.velocity = Vector3.zero;
    //            _navMeshAgent.ResetPath();
    //        }

    //        enemy.fsm.ChangeState(enemy.AttackState);
    //    }

    //    //if (!enemy.isAggroed)
    //    //{
    //    //    enemy.fsm.ChangeState(enemy.SearchState);
    //    //}

    //    //if (enemy.isWhitinCombatRadius)
    //    //{
    //    //    enemy.fsm.ChangeState(enemy.AttackState);
    //    //}


    //    Vector3 direction = (playerTransform.position - transform.position);
    //    direction.y = 0f;
    //    if (direction.sqrMagnitude > 0.01f)
    //    {
    //        Quaternion targetRotation = Quaternion.LookRotation(direction);
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _enemyModel.statsSO.rotationSpeed * Time.deltaTime * _speedAgentMultiply);
    //    }
    //}

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > _enemyModel.statsSO.AttackRange)
        {
            if (_navMeshAgent.isStopped)
                _navMeshAgent.isStopped = false;

            if (coordinator != null)
                targetPos = coordinator.GetFlankPosition(playerTransform, transform, flankRadius);
            else
                targetPos = playerTransform.position;

            _navMeshAgent.SetDestination(targetPos);
        }
        else
        {
            if (!_navMeshAgent.isStopped)
            {
                _navMeshAgent.isStopped = true;
                _navMeshAgent.velocity = Vector3.zero;
                _navMeshAgent.ResetPath();
            }

            enemy.fsm.ChangeState(enemy.AttackState);
        }

        // rotación hacia el jugador
        Vector3 direction = (playerTransform.position - transform.position);
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _enemyModel.statsSO.rotationSpeed * Time.deltaTime * _speedAgentMultiply
            );
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

