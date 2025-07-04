using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dead-Dead Boss", menuName = "Enemy Logic/Dead Logic/Dead Boss")]

public class EnemyDeadBoss : EnemyDeadSOBase
{
    private float _timer;
    private float animationTime = 4f;
    [SerializeField] private string bossMessage= "You... have only delayed the inevitable...";
    [SerializeField] private float messageDuration=3.5f;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
        _bossModel.PrintMessage(bossMessage, messageDuration);

        _bossView.PlayDeathAnimation();
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

    public override void Initialize(GameObject gameObject, IEnemyBaseController enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
