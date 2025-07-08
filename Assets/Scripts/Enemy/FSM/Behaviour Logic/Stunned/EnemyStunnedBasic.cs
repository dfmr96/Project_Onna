using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stunned-Stunned Basic", menuName = "Enemy Logic/Stuned Logic/Stunned Basic")]
 public class EnemyStunnedBasic : EnemyStunnedSOBase
 {
    private float _timer;
    [SerializeField] private float _timeStun = 5f;


    public override void DoEnterLogic()
    {
            base.DoEnterLogic();

            _enemyView.PlayStunnedAnimation();

            _enemyModel.OnDeath += HandleDeathState;
    }

        public override void DoExitLogic()
        {
            base.DoExitLogic();

            _timer = 0f;
            _enemyModel.OnDeath -= HandleDeathState;

    }

    public override void DoFrameUpdateLogic()
        {
            base.DoFrameUpdateLogic();

             _timer += Time.deltaTime;


        if (_timer >= _timeStun)
        {
                    //_enemyView.PlayMovingAnimation(_enemyModel.statsSO.moveSpeed);
                    enemy.fsm.ChangeState(enemy.ChaseState);

            }
    }

        public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
        {
            base.Initialize(gameObject, enemy);
        }

        public override void ResetValues()
        {
            base.ResetValues();
        }


        private void HandleDeathState(EnemyModel enemy_)
        {
            DoExitLogic();
            //_enemyView.PlayDeathAnimation();
        }
}

