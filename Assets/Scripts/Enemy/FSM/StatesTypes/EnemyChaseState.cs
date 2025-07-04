using Player;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyState<BaseEnemyController>
{


    private EnemyChaseSOBase chaseBehavior;


    public EnemyChaseState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemyChaseSOBase chaseBehavior) : base(enemy, fsm)
    {
        this.chaseBehavior = chaseBehavior;

    }

    public override void EnterState()
    {
        base.EnterState();
        //enemy.EnemyPatrolBaseInstance.DoEnterLogic();
        chaseBehavior.DoEnterLogic();


    }

    public override void ExitState()
    {
        base.ExitState();
        //enemy.EnemyPatrolBaseInstance.DoExitLogic();
        chaseBehavior.DoExitLogic();

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        //enemy.EnemyPatrolBaseInstance.DoFrameUpdateLogic();
        chaseBehavior.DoFrameUpdateLogic();
        //Debug.Log("Persiguiendo al jugador hacia: " + PlayerHelper.GetPlayer().transform.position);


    }
}
