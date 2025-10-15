using System.Collections;
using UnityEngine;
using Player;

public class ProjectileBurstShooter : MonoBehaviour
{
    private int minProjectilesPerBurst;
    private int maxProjectilesPerBurst;
    private float delayBetweenShots;
    private float delayBetweenBursts;
    private float spreadAngle;

    private ProjectileSpawner _spawner;
    private BossModel _bossModel;
    private BossController _bossController;
    private BossView _bossView;
    private Transform _playerTransform;

    private Coroutine _loopCoroutine;

    float verticalOffset = 1.0f;

    private void Start()
    {
        _spawner = GameManager.Instance?.projectileSpawner;
        _bossModel = GetComponentInParent<BossModel>();
        _bossController = GetComponentInParent<BossController>();
        _bossView = GetComponentInParent<BossView>();
        _playerTransform = PlayerHelper.GetPlayer().transform;

        minProjectilesPerBurst = _bossModel.statsSO.minProjectilesPerBurst;
        maxProjectilesPerBurst = _bossModel.statsSO.maxProjectilesPerBurst;
        delayBetweenShots = _bossModel.statsSO.delayBetweenShots;
        delayBetweenBursts = _bossModel.statsSO.delayBetweenBursts;
        spreadAngle = _bossModel.statsSO.spreadAngle;
    }

    public void StartBurstLoop()
    {

     

        if (_loopCoroutine == null)
            _loopCoroutine = StartCoroutine(BurstLoop());
    }

    public void StopBurstLoop()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }
    }

    private IEnumerator BurstLoop()
    {
        while (true)
        {
            int shots = Random.Range(minProjectilesPerBurst, maxProjectilesPerBurst + 1);

            for (int i = 0; i < shots; i++)
            {
                ShootProjectile();
                yield return new WaitForSeconds(delayBetweenShots);
            }

            yield return new WaitForSeconds(delayBetweenBursts);
        }
    }

    private void ShootProjectile()
    {
        if (_spawner == null || _playerTransform == null || _bossController?.firePoint == null) return;

        Vector3 targetPos = _playerTransform.position;
        targetPos.y += verticalOffset;

        Vector3 dir = (targetPos - _bossController.firePoint.position).normalized;
        dir = Quaternion.Euler(0, Random.Range(-spreadAngle, spreadAngle), 0) * dir;

        _spawner.SpawnProjectileBoss(
            _bossController.firePoint.position,
            dir,
            _bossModel.statsSO.ShootForce,
            _bossModel.statsSO.ProjectileDamage
        );

        _bossView?.PlayProjectilesAttackAnimation();
        _bossView?.ShootShotgun();
    }
}
