using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeadState : EnemyState
{


   


    public EnemyDeadState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {
   

    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemyDeadBaseInstance.DoEnterLogic();
     

    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemyDeadBaseInstance.DoExitLogic();



    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemyDeadBaseInstance.DoFrameUpdateLogic();


      
    }


}
