using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Dead-Dead Boss", menuName = "Enemy Logic/Dead Logic/Dead Boss")]

public class EnemyDeadBoss : EnemyDeadSOBase
{
    private float _timer;
    [SerializeField] private float animationTime = 11f;
    [SerializeField] private string bossMessage= "You... have only delayed the inevitable...";
    [SerializeField] private float messageDuration=8f;
    [SerializeField] private GameObject particleExplosion;
    [SerializeField] private GameObject loadingScreenPrefab;



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

        GameModeSelector.SelectedMode = GameMode.Hub;
        //SceneManager.LoadScene("HUB");

        SceneManagementUtils.AsyncLoadSceneByName("HUB", loadingScreenPrefab, PlayerHelper.GetPlayer().GetComponent<PlayerController>());

    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        _timer += Time.deltaTime;

        if (_timer > 9.5f)
        {
            if (particleExplosion != null)
            {
                float midHeight = _collider != null ? _collider.height : 1f;
                Vector3 spawnPos = transform.position + Vector3.up * midHeight;
                GameObject particles = Instantiate(particleExplosion, spawnPos, Quaternion.identity);

                Destroy(particles, 1.5f);
            }

            enemy.gameObject.GetComponent<BossController>().PlayAudioDead();
        }

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
