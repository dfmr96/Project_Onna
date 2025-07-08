using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dead-Dead Torret", menuName = "Enemy Logic/Dead Logic/Dead Torret")]

public class EnemyDeadTorret : EnemyDeadSOBase
{
    private float _timer;
    private float animationTime = 0.7f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _timer = 0f;

        _enemyView.PlayDeathParticles();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        _timer += Time.deltaTime;

        if (_timer > animationTime)
        {
            _timer = 0f;
            DoExitLogic();
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
}

