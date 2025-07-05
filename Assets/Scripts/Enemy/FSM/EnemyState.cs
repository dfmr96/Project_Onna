using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState<T> where T : MonoBehaviour
{
    protected T enemy;
    protected EnemyStateMachine<T> fsm;

    public EnemyState(T enemy, EnemyStateMachine<T> fsm)
    {
        this.enemy = enemy;
        this.fsm = fsm;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
}

