using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefendState : EnemyState<BaseEnemyController>
{
    private EnemyDefendSOBase defendBehavior;

    public EnemyDefendState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyDefendSOBase defendBehavior) : base(enemy, fsm)
    {
        this.defendBehavior = defendBehavior;

    }

    public override void EnterState()
    {
        base.EnterState();

        defendBehavior.DoEnterLogic();

    }



    public override void ExitState()
    {
        base.ExitState();

        defendBehavior.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        defendBehavior.DoFrameUpdateLogic();


    }
}
