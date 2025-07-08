using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyState<BaseEnemyController>
{

    private EnemyIdleSOBase idleBehavior;

    public EnemyIdleState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyIdleSOBase idleBehavior) : base(enemy, fsm)
    {
        this.idleBehavior = idleBehavior;
    }

    public override void EnterState()
    {
        base.EnterState();
        idleBehavior.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        idleBehavior.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        idleBehavior.DoFrameUpdateLogic();




    }


}

