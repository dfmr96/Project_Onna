using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-CombinedBurstAndLaser", menuName = "Enemy Logic/Boss Attack Logic/Combined Burst and Laser")]
public class MonarchCombinedBurstAndLaserAttack : EnemyAttackSOBase
{
    [Header("Burst Config")]
    [SerializeField] private int burstDuration = 1;
    [SerializeField] private float timeBetweenBursts = 3f;

    [Header("Laser Config")]
    [SerializeField] private float laserDuration = 4f;
    [SerializeField] private float laserCooldownAfter = 2f;
    [SerializeField] private float rotationMultiply = 15f;
    [SerializeField] private float rotationSmoothness = 5f;

    [Header("Boss Messages")]
    [SerializeField] private float messageDuration = 4f;
    [SerializeField] List<string> bossMessage;



    private enum AttackPhase { Burst, WaitAfterBurst, Laser, WaitAfterLaser }
    private AttackPhase currentPhase;

    private ProjectileBurstShooter _burstShooter;
    private LaserDamage _laser;

    private int shotsRemainingInBurst;
    private float timer;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        int randomIndex = Random.Range(0, bossMessage.Count);
        _bossModel.PrintMessage(bossMessage[randomIndex], messageDuration);

        //_navMeshAgent.isStopped = true;
        _navMeshAgent.updateRotation = true;
        //_navMeshAgent.stoppingDistance = 0f;

        _burstShooter = _bossModel.GetComponentInChildren<ProjectileBurstShooter>();
        _laser = _bossModel.GetComponentInChildren<LaserDamage>(true);

        shotsRemainingInBurst = burstDuration;
        //Comenzar rafaga
        StartBurst();
    }

    private void StartBurst()
    {
        currentPhase = AttackPhase.Burst;
        timer = 0f;
        _laser?.StopLaser();
        _burstShooter?.StartBurstLoop();
    }

    private void StartWaitAfterBurst()
    {
        currentPhase = AttackPhase.WaitAfterBurst;
        timer = 0f;
        _burstShooter?.StopBurstLoop();
    }

    private void StartLaser()
    {
        currentPhase = AttackPhase.Laser;
        timer = 0f;
        _laser.StartLaser();
    }

    private void StartWaitAfterLaser()
    {
        currentPhase = AttackPhase.WaitAfterLaser;
        timer = 0f;
        _laser?.StopLaser();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case AttackPhase.Burst:
                if (shotsRemainingInBurst <= 0)
                {
                    StartWaitAfterBurst();
                    shotsRemainingInBurst = burstDuration;
                }
              
                break;

            case AttackPhase.WaitAfterBurst:
                if (timer >= timeBetweenBursts)
                {
                    StartLaser();
                }
                break;

            case AttackPhase.Laser:
                if (timer >= laserDuration)
                {
                    StartWaitAfterLaser();
                }
                else
                {
                    UpdateLaserRotation();
                }
                break;

            case AttackPhase.WaitAfterLaser:
                if (timer >= laserCooldownAfter)
                {
                    StartBurst();
                }
                break;
        }

        if (currentPhase == AttackPhase.Burst && _burstShooter != null)
        {
            //Simular disparos
            if (timer >= 1f) 
            {
                shotsRemainingInBurst--;
                timer = 0f;
            }
        }
    }

    private void UpdateLaserRotation()
    {
        if (playerTransform == null) return;

        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            _bossView.PlayProjectilesAttackAnimation();

            Quaternion targetRot = Quaternion.LookRotation(dir);
            float rotationSpeed = rotationSmoothness * rotationMultiply;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _burstShooter?.StopBurstLoop();
        _laser?.StopLaser();
        currentPhase = AttackPhase.WaitAfterLaser;
        timer = 0f;
    }
}

