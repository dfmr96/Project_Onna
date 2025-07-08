using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dead-Dead Basic", menuName = "Enemy Logic/Dead Logic/Dead Basic")]

public class EnemyDeadBasic : EnemyDeadSOBase
{
    private float _timer;
    private float animationTime = 4f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _enemyView.PlayDeathAnimation();
        _timer = 0f;
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

    public override void Initialize(GameObject gameObject, EnemyController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}

