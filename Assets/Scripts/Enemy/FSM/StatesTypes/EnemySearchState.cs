using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchState : EnemyState
{
    private Transform _playerTransform;
    private NavMeshAgent _navMeshAgent;

    private float _searchTimer;
    private float _maxSearchTime = 10f;
    private Vector3 _lastKnownPosition;
    private float _searchRadius = 7f; 
    private float _patrolRadius = 6f; 
    private float _patrolTime = 2f;

    private float _patrolTimer;
    private bool _isPatrolling;

    public EnemySearchState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
        //_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //_playerTransform = PlayerHelper.GetPlayer().transform;

        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        base.EnterState();
        _playerTransform = PlayerHelper.GetPlayer().transform;

        _lastKnownPosition = _playerTransform.position; 
        _navMeshAgent.SetDestination(_lastKnownPosition);

        _searchTimer = 0f; 
        _patrolTimer = 0f; 
        _isPatrolling = false;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        _searchTimer += Time.deltaTime;

        if (_searchTimer > _maxSearchTime && !_isPatrolling)
        {
            StartPatrol();
        }

        if (_isPatrolling)
        {
            _patrolTimer += Time.deltaTime;

            //Busca caminado lento
            _navMeshAgent.speed = 0.9f;

            if (_patrolTimer > _patrolTime)
            {
                fsm.ChangeState(enemy.PatrolState); 
            }
        }

        if (Vector3.Distance(_playerTransform.position, enemy.transform.position) < _searchRadius)
        {
            fsm.ChangeState(enemy.ChaseState);
        }

        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !_isPatrolling)
        {
            StartPatrol();
        }
    }

    private void StartPatrol()
    {
        _isPatrolling = true;
        _lastKnownPosition = _playerTransform.position;

        Vector3 randomPatrolPoint = GetRandomPointInRadius(_lastKnownPosition, _patrolRadius);
        _navMeshAgent.SetDestination(randomPatrolPoint);
    }

    private Vector3 GetRandomPointInRadius(Vector3 center, float radius)
    {
        Vector3 randomPoint = center + (Random.insideUnitSphere * radius);
        randomPoint.y = center.y; 
        return randomPoint;
    }
}
