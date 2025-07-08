using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.AI;

public class EnemyEscapeSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;
    protected EnemyView _enemyView;
    protected EnemyModel _enemyModel;


    protected NavMeshAgent _navMeshAgent;

    [Header("Escape Settings")]
    [SerializeField] protected float escapeDistance = 3f;
    [SerializeField] protected float desiredDistance = 6f;

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

