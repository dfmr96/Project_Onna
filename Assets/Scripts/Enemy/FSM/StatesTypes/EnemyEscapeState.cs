using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEscapeState : EnemyState<BaseEnemyController>
{


    private EnemyEscapeSOBase escapeBehavior;



    public EnemyEscapeState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyEscapeSOBase escapeBehavior) : base(enemy, fsm)
    {
        this.escapeBehavior = escapeBehavior;

    }

    public override void EnterState()
    {
        base.EnterState();
        escapeBehavior.DoEnterLogic();


    }

    public override void ExitState()
    {
        base.ExitState();
        escapeBehavior.DoExitLogic();



    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        escapeBehavior.DoFrameUpdateLogic();



    }


}
