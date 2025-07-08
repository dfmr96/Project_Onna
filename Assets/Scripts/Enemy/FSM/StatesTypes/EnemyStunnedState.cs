using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStunnedState : EnemyState
{



    public EnemyStunnedState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        enemy.EnemyStunnedBaseInstance.DoEnterLogic();

    }



    public override void ExitState()
    {
        base.ExitState();

        enemy.EnemyStunnedBaseInstance.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.EnemyStunnedBaseInstance.DoFrameUpdateLogic();


    }


}
