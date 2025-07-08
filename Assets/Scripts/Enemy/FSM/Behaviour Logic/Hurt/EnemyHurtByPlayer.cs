using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Hurt-Hurt By Player Attack", menuName = "Enemy Logic/Hurt Logic/Hurt By Player Attack")]
 public class EnemyHurtByPlayer : EnemyHurtSOBase
 {
    private float _timer;


    public override void DoEnterLogic()
        {
            base.DoEnterLogic();

        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();
        _navMeshAgent.velocity = Vector3.zero;

        _enemyView.PlayDamageAnimation();

        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();

            ResetValues();

            enemy.fsm.ChangeStateDirect(enemy.SearchState);
    }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();

             _timer += Time.deltaTime;

            if (_timer >= _timeHurt)
            {
                ResetValues();
                enemy.fsm.ChangeState(enemy.ChaseState);

            }
    }

        public override void Initialize(GameObject gameObject, EnemyController enemy)
        {
            base.Initialize(gameObject, enemy);
        }

        public override void ResetValues()
        {
            base.ResetValues();
            _timer = 0f;
            _navMeshAgent.isStopped = false;

    }
}

