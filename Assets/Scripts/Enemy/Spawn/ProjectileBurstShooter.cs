using System.Collections;
using UnityEngine;
using Player;
using System.Collections.Generic;

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
    private List<EnemyProjectile> _pendingProjectiles = new List<EnemyProjectile>();


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
        if (_pendingProjectiles.Count > 0)
        {
            foreach (var proj in _pendingProjectiles)
            {
                if (proj != null) proj.gameObject.SetActive(false);
            }
            _pendingProjectiles.Clear();
        }

        if (_loopCoroutine == null)
            _loopCoroutine = StartCoroutine(BurstLoop());

        AnimationSpawnProjectileFunc(1);

    }

    public void StopBurstLoop()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        // limpiar proyectiles pendientes cuando se corta el ataque
        if (_pendingProjectiles.Count > 0)
        {
            foreach (var proj in _pendingProjectiles)
            {
                if (proj != null)
                {
                    // si usás pooling, devolverlo al pool
                    proj.gameObject.SetActive(false);
                    // o si no, destruir directamente:
                    // Destroy(proj.gameObject);
                }
            }

            _pendingProjectiles.Clear();
        }
    }

    private IEnumerator BurstLoop()
    {
        while (true)
        {
            int shots = Random.Range(minProjectilesPerBurst, maxProjectilesPerBurst + 1);

            for (int i = 0; i < shots; i++)
            {
    
                // Dispara solo uno (el último de la lista)
                AnimationShootProjectileFunc(1);

                yield return new WaitForSeconds(delayBetweenShots);

                AnimationSpawnProjectileFunc(1);

            }

            yield return new WaitForSeconds(delayBetweenBursts);
        }
    }



    public void AnimationSpawnProjectileFunc(int amount = 1)
    {
        Transform firePoint = _bossController.firePoint;

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = firePoint.position;
            Quaternion spawnRot = firePoint.rotation;

            EnemyProjectile pendingProjectile = _spawner.SpawnIdleProjectile(spawnPos, spawnRot);

            if (pendingProjectile != null)
            {
                _pendingProjectiles.Add(pendingProjectile);

                // Efecto visual de aparición
                var fx = pendingProjectile.GetComponent<ObjectScalerFx>();
                if (fx != null)
                    fx.enabled = true;
            }
        }
    }


    public void AnimationShootProjectileFunc(int amount = 1)
    {
        if (_pendingProjectiles.Count == 0) return;
        if (_spawner == null || _playerTransform == null || _bossController?.firePoint == null) return;

        for (int i = 0; i < amount && _pendingProjectiles.Count > 0; i++)
        {
            var projectile = _pendingProjectiles[0]; // saca el primero
            _pendingProjectiles.RemoveAt(0);

            if (projectile == null) continue;

            Vector3 targetPos = _playerTransform.position;
            targetPos.y += verticalOffset;

            Vector3 dir = (targetPos - _bossController.firePoint.position).normalized;
            dir = Quaternion.Euler(0, Random.Range(-spreadAngle, spreadAngle), 0) * dir;

            projectile.FireBoss(dir, _bossModel.statsSO.ShootForce, _bossModel.statsSO.ProjectileDamage, _bossModel);
        }

        _bossView?.PlayProjectilesAttackAnimation();
        _bossView?.ShootShotgun();
    }



}
