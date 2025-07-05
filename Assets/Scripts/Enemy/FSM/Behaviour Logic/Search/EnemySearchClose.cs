using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Search-Search In Random Points", menuName = "Enemy Logic/Search Logic/Search In Random Points Near")]

public class EnemySearchClose : EnemySearchSOBase
{

    private float _searchTimer;
    private float _maxSearchTime = 10f;
    private Vector3 _lastKnownPosition;
    private float _searchRadius = 7f;
    private float _patrolRadius = 6f;
    private float _patrolTime = 2f;

    private float _patrolTimer;
    private bool _isPatrolling;

    public override void DoEnterLogic()
        {
            base.DoEnterLogic();


        _lastKnownPosition = playerTransform.position;
        _navMeshAgent.SetDestination(_lastKnownPosition);

        _searchTimer = 0f;
        _patrolTimer = 0f;
        _isPatrolling = false;

    }

    public override void DoExitLogic()
        {
            base.DoExitLogic();

            ResetValues();


        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();

        _searchTimer += Time.deltaTime;

        if (_searchTimer > _maxSearchTime && !_isPatrolling)
        {
            StartPatrol();
        }

        if (_isPatrolling)
        {
            _patrolTimer += Time.deltaTime;

            _navMeshAgent.speed = 0.9f; //Busca caminando lento

            if (_patrolTimer > _patrolTime)
            {
                enemy.fsm.ChangeState(enemy.PatrolState);
                return;
            }
        }

        if (Vector3.Distance(playerTransform.position, enemy.transform.position) < _searchRadius)
        {
            enemy.fsm.ChangeState(enemy.ChaseState);
            return;
        }

        if (!_isPatrolling && !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            StartPatrol();
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

  

    private void HandleHealthChanged(float currentHealth)
    {
        //Si es lastimado durante la patrulla, pasa a perseguir al player

        enemy.fsm.ChangeState(enemy.ChaseState);

    }

    private void StartPatrol()
    {
        _isPatrolling = true;
        _patrolTimer = 0f;

        Vector3 randomPatrolPoint = GetRandomPointInRadius(_lastKnownPosition, _patrolRadius);
        _navMeshAgent.SetDestination(randomPatrolPoint);
    }

    private Vector3 GetRandomPointInRadius(Vector3 center, float radius)
    {
        Vector3 randomPoint = center + (Random.insideUnitSphere * radius);
        randomPoint.y = center.y; // Mantener altura constante
        return randomPoint;
    }
}
