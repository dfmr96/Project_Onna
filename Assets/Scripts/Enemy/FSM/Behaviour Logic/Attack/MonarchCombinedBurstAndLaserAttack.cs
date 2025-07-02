using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-CombinedBurstAndLaser", menuName = "Enemy Logic/Boss Attack Logic/Combined Burst and Laser")]
public class MonarchCombinedBurstAndLaserAttack : EnemyAttackSOBase
{
    [SerializeField] private int minBurstShots = 4;
    [SerializeField] private int maxBurstShots = 6;
    [SerializeField] private float timeBetweenBursts = 3f;
    [SerializeField] private float laserDuration = 4f;
    [SerializeField] private float laserCooldownAfter = 2f;

    private enum AttackPhase { Burst, WaitAfterBurst, Laser, WaitAfterLaser }
    private AttackPhase currentPhase;

    private ProjectileBurstShooter _burstShooter;
    private LaserDamage _laser;

    private int shotsRemainingInBurst;
    private float timer;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        _navMeshAgent.isStopped = true;
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.stoppingDistance = 0f;

        _burstShooter = _bossModel.GetComponentInChildren<ProjectileBurstShooter>();
        _laser = _bossModel.GetComponentInChildren<LaserDamage>(true);

        // Comenzar con ráfaga
        StartBurst();
    }

    private void StartBurst()
    {
        currentPhase = AttackPhase.Burst;
        shotsRemainingInBurst = Random.Range(minBurstShots, maxBurstShots + 1);
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
        _laser?.StartLaser();
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
                // Aquí puedes controlar que la ráfaga dispare un tiro a la vez
                // y se reduzca shotsRemainingInBurst, por ejemplo
                // Asumo que ProjectileBurstShooter maneja eso solo
                if (shotsRemainingInBurst <= 0)
                {
                    StartWaitAfterBurst();
                }
                else
                {
                    // Aquí podrías detectar cuando se dispara un tiro para decrementar
                    // Pero si el _burstShooter controla su lógica internamente,
                    // podrías hacer algo como:
                    // shotsRemainingInBurst = 0; // forzar terminar tras un loop
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
                    // Actualizar rotación hacia jugador con delay o suavizado si quieres
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

        // Si quieres controlar shotsRemainingInBurst basado en el shooter, aquí podrías:
        if (currentPhase == AttackPhase.Burst && _burstShooter != null)
        {
            // Ejemplo: si el shooter tiene evento o variable pública que indica tiros restantes
            // Aquí solo decrementa manualmente para simular
            if (timer >= 1f) // ejemplo para simular disparos cada 1s
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
            float rotationSmoothness = 2f;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmoothness * Time.deltaTime);
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

