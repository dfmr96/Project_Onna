using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Stunned-Stunned Basic", menuName = "Enemy Logic/Stuned Logic/Stunned Basic")]
 public class EnemyStunnedBasic : EnemyStunnedSOBase
 {
    private float _timer;
    [SerializeField] private float _timeStun = 5f;
    [SerializeField] private GameObject particleStun;
    private GameObject particles;


    public override void DoEnterLogic()
    {
            base.DoEnterLogic();


            Vector3 spawnPos = transform.position + Vector3.up * 1.8f;
            particles = Instantiate(particleStun, spawnPos, Quaternion.Euler(-90f,0f,0f));

            
            _enemyView.PlayStunnedAnimation();

            _enemyModel.OnDeath += HandleDeathState;
            _enemyModel.OnHealthChanged += HandleHealthChanged;

    }

    public override void DoExitLogic()
        {
            base.DoExitLogic();

            _timer = 0f;
            _enemyModel.OnDeath -= HandleDeathState;
            _enemyModel.OnHealthChanged -= HandleHealthChanged;

        Destroy(particles);

        _enemyView.ResetStunnedAnimation();

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

        private void HandleHealthChanged(float currentHealth)
        {

        //enemy.fsm.ChangeState(enemy.SearchState);



    }
}

