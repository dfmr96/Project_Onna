using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtState : EnemyState
{
    public EnemyHurtState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        enemy.EnemyHurtBaseInstance.DoEnterLogic();

    }



    public override void ExitState()
    {
        base.ExitState();

        enemy.EnemyHurtBaseInstance.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.EnemyHurtBaseInstance.DoFrameUpdateLogic();


    }
}
