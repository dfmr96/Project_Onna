using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Idle-Tank with Shield activation", menuName = "Enemy Logic/Idle Logic/Tank with Shield activation")]

public class EnemyIdleTank : EnemyIdleSOBase
{
    [SerializeField] private float duration = 2f;
    private float timer = 0f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        //enemy.SetShield(false);

        timer = 0f;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        ResetValues();
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
        enemy.SetShield(true);


    }

}
