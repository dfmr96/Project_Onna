using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchState : EnemyState<BaseEnemyController>
{
    private EnemySearchSOBase searchBehavior;

    private Transform _playerTransform;
    private NavMeshAgent _navMeshAgent;



    //Casteo seguro para acceder a EnemyController especifico
    private EnemyController Enemy => (EnemyController)enemy;

    public EnemySearchState(BaseEnemyController enemy, EnemyStateMachine<BaseEnemyController> fsm, EnemySearchSOBase searchBehavior) : base(enemy, fsm)
    {
        this.searchBehavior = searchBehavior;
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void EnterState()
    {
        base.EnterState();


       

        searchBehavior.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        searchBehavior.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

       

        searchBehavior.DoFrameUpdateLogic();
    }

   
}
