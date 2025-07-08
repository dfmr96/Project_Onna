using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyStunnedSOBase : ScriptableObject
{
    protected IEnemyBaseController enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    protected EnemyView _enemyView;
    protected EnemyModel _enemyModel;

    private NavMeshAgent _navMeshAgent;

    public virtual void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        this.gameObject = gameObject;
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = PlayerHelper.GetPlayer().transform;
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

    }

    public virtual void DoEnterLogic()
    {
        _enemyView = gameObject.GetComponent<EnemyView>();
        _enemyModel = gameObject.GetComponent<EnemyModel>();    

    }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic()
    {

    }
    public virtual void ResetValues()
    {

    }
}

