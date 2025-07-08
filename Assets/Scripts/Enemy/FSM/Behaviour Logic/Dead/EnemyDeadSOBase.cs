using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyDeadSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected EnemyView _enemyView;
    protected CapsuleCollider _collider;

    protected Transform playerTransform;

    private NavMeshAgent _navMeshAgent;

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
        _enemyView = enemy.GetComponent<EnemyView>();
        _collider = enemy.GetComponent<CapsuleCollider>();

        _navMeshAgent.speed = 0;
        _navMeshAgent.isStopped = true;
        _collider.enabled = false;

    }
    public virtual void DoExitLogic() { 
        ResetValues();
        DeathManager.Instance.DestroyObject(enemy.gameObject);


    }
    public virtual void DoFrameUpdateLogic()
    {

    }
    public virtual void ResetValues()
    {

    }
}

