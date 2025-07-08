using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEscapeState : EnemyState
{





    public EnemyEscapeState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {


    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemyEscapeBaseInstance.DoEnterLogic();


    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemyEscapeBaseInstance.DoExitLogic();



    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemyEscapeBaseInstance.DoFrameUpdateLogic();



    }


}
