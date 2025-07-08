using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyChaseSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected EnemyModel _enemyModel;

    protected Transform playerTransform;

    protected NavMeshAgent _navMeshAgent;

    [Header("Chase Settings")]
    [SerializeField][Range(0f, 50f)] protected float minDistanceToPlayer = 5f;


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
