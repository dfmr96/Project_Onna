using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState<BaseEnemyController>
{

    private EnemyPatrolSOBase patrolBehavior;


    public EnemyPatrolState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyPatrolSOBase patrolBehavior) : base(enemy, fsm)
    {
        this.patrolBehavior = patrolBehavior;

    }

    public override void EnterState()
    {
        base.EnterState();
        //enemy.EnemyPatrolBaseInstance.DoEnterLogic();
        patrolBehavior.DoEnterLogic();

    }

    public override void ExitState()
    {
        base.ExitState();
        //enemy.EnemyPatrolBaseInstance.DoExitLogic();
        patrolBehavior.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        //enemy.EnemyPatrolBaseInstance.DoFrameUpdateLogic();
        patrolBehavior.DoFrameUpdateLogic();

    }


}
