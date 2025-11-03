using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Idle-Still In Place", menuName = "Enemy Logic/Idle Logic/Still In Place")]

public class EnemyIdleJustStill : EnemyIdleSOBase
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool onlyWaiting = true;

    private float timer = 0f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        timer = 0f;
        enemy.SetShield(true);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        enemy.SetShield(false);
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        timer += Time.deltaTime;

        if (timer > duration && onlyWaiting)
        {
            //float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            //if (distanceToPlayer > model.statsSO.AttackRange)
            //{
            //    enemy.fsm.ChangeState(enemy.ChaseState);

            //}
            //else
            //{
                enemy.fsm.ChangeState(enemy.ChaseState);

            //}

        }
        else
        {
            if (enemy.isWhitinCombatRadius)
            {
                enemy.fsm.ChangeState(enemy.AttackState);
            }
        }



    }

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        _navMeshAgent.isStopped = false;

    }

}
