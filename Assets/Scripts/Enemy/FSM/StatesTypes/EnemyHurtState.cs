using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtState : EnemyState<BaseEnemyController>
{
    private EnemyHurtSOBase hurtBehavior;

    public EnemyHurtState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyHurtSOBase hurtBehavior) : base(enemy, fsm)
    {
        this.hurtBehavior = hurtBehavior;
    }

    public override void EnterState()
    {
        base.EnterState();

        hurtBehavior.DoEnterLogic();

    }



    public override void ExitState()
    {
        base.ExitState();

        hurtBehavior.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        hurtBehavior.DoFrameUpdateLogic();


    }
}
