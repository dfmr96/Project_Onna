using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyDeadSOBase : ScriptableObject
{
    protected IEnemyBaseController enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected EnemyView _enemyView;
    protected EnemyModel _enemyModel;
    protected CapsuleCollider _collider;
    protected BossModel _bossModel;
    protected BossView _bossView;

    protected Transform playerTransform;

    private NavMeshAgent _navMeshAgent;

    public virtual void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        this.gameObject = gameObject;
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = PlayerHelper.GetPlayer().transform;
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        _bossModel = gameObject.GetComponent<BossModel>();
        _bossView = gameObject.GetComponent<BossView>();
        _enemyModel = gameObject.GetComponent<EnemyModel>();
    }

    public virtual void DoEnterLogic()
    {
        _enemyView = gameObject.GetComponent<EnemyView>();
        _collider = gameObject.GetComponent<CapsuleCollider>();
        _bossModel = gameObject.GetComponent<BossModel>();
        _bossView = gameObject.GetComponent<BossView>();
        _enemyModel = gameObject.GetComponent<EnemyModel>();

        _navMeshAgent.speed = 0;
        //_navMeshAgent.isStopped = true;
        _collider.enabled = false;

    }
    public virtual void DoExitLogic() { 
        ResetValues();

        //Si tiene el buff de explotar 
        if(_enemyModel != null)
        {
            if (_enemyModel.variantSO.explodesOnDeath)
            {
                EnemyManager.Instance.InstantiateMutantDeath(transform, _enemyModel.variantSO.explosionLifetime);
            }
        }


        EnemyManager.Instance.DestroyObject(enemy.gameObject);


    }
    public virtual void DoFrameUpdateLogic()
    {

    }
    public virtual void ResetValues()
    {

    }
}

