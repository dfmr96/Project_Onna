using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyPatrolSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    protected NavMeshAgent _navMeshAgent;

    protected Vector3 _targetPos;
    protected EnemyModel _enemyModel;

    public virtual void Initialize(GameObject gameObject, EnemyController enemy)
    {
        this.gameObject = gameObject;
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = PlayerHelper.GetPlayer().transform;
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();

    }

    public virtual void DoEnterLogic()
    {
        _enemyModel = enemy.GetComponent<EnemyModel>();
        _navMeshAgent.isStopped = false;

    }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic()
    {

        if (enemy.isAggroed)
        {
            enemy.fsm.ChangeState(enemy.ChaseState);
        }

        if (playerTransform == null)
        {
            enemy.fsm.ChangeState(enemy.IdleState);
            return;
        }

    }
    public virtual void ResetValues()
    {

    }
}
