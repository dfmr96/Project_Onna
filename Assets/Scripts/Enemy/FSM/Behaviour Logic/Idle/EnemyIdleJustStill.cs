using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Idle-Still In Place", menuName = "Enemy Logic/Idle Logic/Still In Place")]

public class EnemyIdleJustStill : EnemyIdleSOBase
{
    [SerializeField] private float duration = 2f;
    private float timer = 0f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();

        timer = 0f;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        timer += Time.deltaTime;

        if (timer > duration)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > model.statsSO.AttackRange)
            {
                enemy.fsm.ChangeState(enemy.ChaseState);

            }
            else
            {
                enemy.fsm.ChangeState(enemy.AttackState);

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
        _navMeshAgent.isStopped = false;

    }

}
