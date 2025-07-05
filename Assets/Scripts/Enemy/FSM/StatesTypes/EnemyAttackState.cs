using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyState<BaseEnemyController>
{
    public EnemyAttackState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm) : base(enemy, fsm)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        // Delegamos la lógica al ScriptableObject actual según la fase
        enemy.CurrentAttackSO?.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.CurrentAttackSO?.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.CurrentAttackSO?.DoFrameUpdateLogic();
    }
}






