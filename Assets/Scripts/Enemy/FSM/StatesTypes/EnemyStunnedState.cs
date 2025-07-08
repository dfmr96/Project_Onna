using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStunnedState : EnemyState<BaseEnemyController>
{
    private EnemyStunnedSOBase stunnedBehavior;

    public EnemyStunnedState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyStunnedSOBase stunnedBehavior) : base(enemy, fsm)
    {
        this.stunnedBehavior = stunnedBehavior;
    }

    public override void EnterState()
    {
        base.EnterState();
        stunnedBehavior.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        stunnedBehavior.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        stunnedBehavior.DoFrameUpdateLogic();
    }
}
