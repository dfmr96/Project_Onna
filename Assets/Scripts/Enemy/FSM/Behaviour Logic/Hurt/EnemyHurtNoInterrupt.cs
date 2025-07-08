using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Hurt-Hurt but no interrupt", menuName = "Enemy Logic/Hurt Logic/Hurt but no interrupt")]
 public class EnemyHurtNoInterrupt : EnemyHurtSOBase
 {
    private float _timer;


    public override void DoEnterLogic()
        {
            base.DoEnterLogic();

        _enemyView.PlayDamageAnimation();

        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();

            ResetValues();

    }

        public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();

             _timer += Time.deltaTime;

            if (_timer >= _timeHurt)
            {
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

    }
}

