using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyBaseController
{
    EnemyStateMachine<BaseEnemyController> fsm { get; }

    //Estados usados por SOBase y EnemyStates
    EnemyChaseState ChaseState { get; }
    EnemyIdleState IdleState { get; }
    EnemyAttackState AttackState { get; }
    EnemyPatrolState PatrolState { get; }
    EnemySearchState SearchState { get; }
    EnemyStunnedState StunnedState { get; }
    EnemyDeadState DeadState { get; }
    EnemyEscapeState EscapeState { get; }
    EnemyHurtState HurtState { get; }
    EnemyDefendState DefendState { get; }

    //Informacion general
    bool isAggroed { get; }
    bool isWhitinCombatRadius { get; }
    GameObject gameObject { get; }
    Transform transform { get; }

    //Para que el ataque funcione desde el SO
    EnemyAttackSOBase CurrentAttackSO { get; }

    //el SO pueda llamar al ataque real
    void ExecuteAttack(IDamageable target);
    void SetShield(bool isOn);
    Rigidbody Rb { get; }

}

