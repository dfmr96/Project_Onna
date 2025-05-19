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

    public virtual void Initialize(GameObject gameObject, EnemyController enemy)
    {
        this.gameObject = gameObject;
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = playerTransform = PlayerHelper.GetPlayer().transform;
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();

    }

    public virtual void DoEnterLogic()
    {
        _enemyModel = enemy.GetComponent<EnemyModel>();

    }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic()
    {

    }
    public virtual void ResetValues()
    {

    }
}
