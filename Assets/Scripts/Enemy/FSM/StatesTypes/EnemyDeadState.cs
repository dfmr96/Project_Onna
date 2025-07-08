using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeadState : EnemyState<BaseEnemyController>
{


    private EnemyDeadSOBase deadBehavior;



    public EnemyDeadState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyDeadSOBase deadBehavior) : base(enemy, fsm)
    {
   
        this.deadBehavior = deadBehavior;
    }

 

    public override void EnterState()
    {
        base.EnterState();
        deadBehavior.DoEnterLogic();
     

    }

    public override void ExitState()
    {
        base.ExitState();
        deadBehavior.DoExitLogic();



    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        deadBehavior.DoFrameUpdateLogic();


      
    }


}
