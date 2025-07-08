using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyState
{



    public EnemyPatrolState(EnemyController enemy, EnemyStateMachine fsm) : base(enemy, fsm)
    {

    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemyPatrolBaseInstance.DoEnterLogic();



     

    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemyPatrolBaseInstance.DoExitLogic();




    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemyPatrolBaseInstance.DoFrameUpdateLogic();

    }

   
}
