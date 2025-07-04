using System.Collections;
using UnityEngine;
using Player;

public class ProjectileBurstShooter : MonoBehaviour
{
    [SerializeField] private int minProjectilesPerBurst = 3;
    [SerializeField] private int maxProjectilesPerBurst = 5;
    [SerializeField] private float delayBetweenShots = 0.2f;
    [SerializeField] private float delayBetweenBursts = 2f;
    [SerializeField] private float spreadAngle = 15f;

    private ProjectileSpawner _spawner;
    private BossModel _bossModel;
    private BossController _bossController;
    private BossView _bossView;
    private Transform _playerTransform;

    private Coroutine _loopCoroutine;

    private void Start()
    {
        _spawner = GameManager.Instance?.projectileSpawner;
        _bossModel = GetComponentInParent<BossModel>();
        _bossController = GetComponentInParent<BossController>();
        _bossView = GetComponentInParent<BossView>();
        _playerTransform = PlayerHelper.GetPlayer().transform;
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
        targetPos.y = _bossController.firePoint.position.y;

        Vector3 dir = (targetPos - _bossController.firePoint.position).normalized;
        dir = Quaternion.Euler(0, Random.Range(-spreadAngle, spreadAngle), 0) * dir;

        _spawner.SpawnProjectile(
            _bossController.firePoint.position,
            dir,
            _bossModel.statsSO.ShootForce,
            _bossModel.statsSO.AttackDamage
        );

        _bossView?.PlayProjectilesAttackAnimation();
    }
}
