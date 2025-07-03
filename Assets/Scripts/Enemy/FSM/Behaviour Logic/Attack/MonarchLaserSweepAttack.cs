using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-LaserSweep", menuName = "Enemy Logic/Boss Attack Logic/Laser Sweep")]
public class MonarchLaserSweepAttack : EnemyAttackSOBase
{
    [SerializeField] private float laserDuration = 5f;
    [SerializeField] private float laserCooldown = 2f;

    [SerializeField] private float rotationSmoothness = 2f;       
    [SerializeField] private float trackingDelay = 0.5f;        
    [SerializeField] private float messageDuration = 4f;
    [SerializeField] List<string> bossMessage;

    private LaserDamage _laser;
    private float _elapsedTime;
    private bool _firing;

    private Vector3 _delayedTargetPosition;
    private float _trackingTimer;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        int randomIndex = Random.Range(0, bossMessage.Count);
        _bossModel.PrintMessage(bossMessage[randomIndex], messageDuration);

        isLookingPlayer = false;
        _laser = _bossModel.GetComponentInChildren<LaserDamage>(true);
        _laser.StartLaser();

        _elapsedTime = 0f;
        _firing = true;
        _trackingTimer = 0f;

        _navMeshAgent.isStopped = true;

        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.y = 0f;

        _delayedTargetPosition = playerTransform.position;
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        _elapsedTime += Time.deltaTime;
        _trackingTimer += Time.deltaTime;

        if (_firing)
        {
            //Actualizar la posición objetivo cada cierto tiempo (delay simulado)
            if (_trackingTimer >= trackingDelay)
            {
                _delayedTargetPosition = playerTransform.position;
                _trackingTimer = 0f;
            }

            // Rotar suavemente hacia la posición retrasada del jugador
            Vector3 dir = _delayedTargetPosition - transform.position;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSmoothness * Time.deltaTime);
            }

            // Fin del disparo
            if (_elapsedTime >= laserDuration)
            {
                _laser.StopLaser();
                _firing = false;
                _elapsedTime = 0f;
            }
        }
        else
        {
            // Cooldown entre disparos
            if (_elapsedTime >= laserCooldown)
            {
                // Girar bruscamente hacia el jugador antes de comenzar otro disparo
                Vector3 toPlayer = playerTransform.position - transform.position;
                toPlayer.y = 0f;

                if (toPlayer != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(toPlayer);
                    transform.rotation = lookRotation;
                }

                DoEnterLogic(); 
            }
        }
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        _laser.StopLaser();
        _firing = false;
        _elapsedTime = 0f;
    }
}
