using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Melee Basic", menuName = "Enemy Logic/Attack Logic/Melee Attack")]
public class EnemyAttackMelee : EnemyAttackSOBase
{
    private bool _hasAttackedOnce = false;


    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _enemyModel.OnHealthChanged += HandleHealthChanged;

        _navMeshAgent.SetDestination(playerTransform.position);
        _hasAttackedOnce = false;

    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        ResetValues();
    }

    public override void DoFrameUpdateLogic()
    {

        base.DoFrameUpdateLogic();

        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer > _distanceToCountExit)
        {
            enemy.fsm.ChangeState(enemy.SearchState);
            return;
        }

        if (!_hasAttackedOnce)
        {
            if (_timer >= _initialAttackDelay)
            {
                Attack();
                _hasAttackedOnce = true;
                _timer = 0f;
            }
        }
        else if (_timer >= _timeBetweenAttacks)
        {
            TriggerAttackColorEffect();

            Attack();
            _timer = 0f;
        }


    }

    public override void Initialize(GameObject gameObject, EnemyController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();

        _enemyView.PlayAttackAnimation(false);
        _hasAttackedOnce = false;
        _enemyModel.OnHealthChanged -= HandleHealthChanged;


    }


    private void HandleHealthChanged(float currentHealth)
    {
        //Si es lastimado dentro del umbral de tiempo, se stunea
        if (_timer >= _initialAttackDelay)
        {
            enemy.fsm.ChangeState(enemy.StunnedState);

        }

    }


    private void Attack()
    {

        if (playerTransform != null)
        {
            distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

            //Si se alejo no aplicar dano
            if (distanceToPlayer <= _distanceToCountExit)
            {
                _enemyView.PlayAttackAnimation(true);
                TriggerAttackColorEffect();
            }
        }


    }


}
