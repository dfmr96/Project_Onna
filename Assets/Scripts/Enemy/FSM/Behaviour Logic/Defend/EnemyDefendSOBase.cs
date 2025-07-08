using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyDefendSOBase : ScriptableObject
{

    protected EnemyController enemy;
    protected EnemyModel enemyModel;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    protected NavMeshAgent _navMeshAgent;
    protected float initialSpeed;

    [Header("Tank Behavior Settings")]
    [SerializeField] protected float minDefendTime = 2f;
    [SerializeField] protected float maxDefendTime = 5f;

    public virtual void Initialize(GameObject gameObject, EnemyController enemy)
    {
        this.gameObject = gameObject;   
        this.enemy = enemy;
        transform = gameObject.transform;

        playerTransform = PlayerHelper.GetPlayer().transform;
        _navMeshAgent = enemy.GetComponent<NavMeshAgent>();


    }

    public virtual void DoEnterLogic() {

        initialSpeed = _navMeshAgent.speed;
        _navMeshAgent.speed = 0;
        _navMeshAgent.isStopped = true;
    }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() { 
    
      
    
    
    }
    public virtual void ResetValues() {

        _navMeshAgent.speed = initialSpeed;
        _navMeshAgent.isStopped = false;
    }
}
