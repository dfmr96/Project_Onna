using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefendState : EnemyState
{
    public EnemyDefendState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        enemy.EnemyDefendBaseInstance.DoEnterLogic();

    }



    public override void ExitState()
    {
        base.ExitState();

        enemy.EnemyDefendBaseInstance.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.EnemyDefendBaseInstance.DoFrameUpdateLogic();


    }
}
