using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol-Patrol In Random Points", menuName = "Enemy Logic/Patrol Logic/Patrol In Random Points Near")]

public class EnemyPatrolInRandomPoints : EnemyPatrolSOBase
    {

    protected float initialSpeed;
    private float walkSpeed = 2f;
    public override void DoEnterLogic()
        {
            base.DoEnterLogic();

            _enemyModel.OnHealthChanged += HandleHealthChanged;

        initialSpeed = _enemyModel.statsSO.moveSpeed;

        //Se obliga a que el enemigo camine
        _navMeshAgent.speed = initialSpeed - walkSpeed;
        _navMeshAgent.angularSpeed = _enemyModel.statsSO.rotationSpeed;

        _targetPos = GetRandomPointInSphere();

        _navMeshAgent.SetDestination(_targetPos);
        _navMeshAgent.isStopped = false;

    }

    public override void DoExitLogic()
        {
            base.DoExitLogic();

            ResetValues();


        }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();


        //check if enemy approach target
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _targetPos = GetRandomPointInSphere();
            _navMeshAgent.SetDestination(_targetPos);
        }


    }

        public override void Initialize(GameObject gameObject, EnemyController enemy)
        {
            base.Initialize(gameObject, enemy);
        }

        public override void ResetValues()
        {
            base.ResetValues();
            _enemyModel.OnHealthChanged -= HandleHealthChanged;
            _navMeshAgent.speed = initialSpeed;
    }

    private Vector3 GetRandomPointInSphere()
    {
        Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * _enemyModel.statsSO.RandomMovementRange;
        randomPoint.y = enemy.transform.position.y;
        return enemy.transform.position + randomPoint;
    }

    private void HandleHealthChanged(float currentHealth)
    {
        //Si es lastimado durante la patrulla, pasa a perseguir al player

        enemy.fsm.ChangeState(enemy.ChaseState);

    }
}
