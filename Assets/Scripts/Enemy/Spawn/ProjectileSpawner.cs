using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private Transform poolParent;

    private ObjectPool<EnemyProjectile> projectilePool;

    private void Awake()
    {
        projectilePool = new ObjectPool<EnemyProjectile>(projectilePrefab, initialPoolSize, poolParent);
    }

    public void SpawnProjectile(Vector3 spawnPos, Vector3 direction, float shootForce, float damage, EnemyModel owner)
    {
        EnemyProjectile proj = projectilePool.Get();
        if (proj == null)
        {
            Debug.LogWarning("[ProjectileSpawner] No projectiles available in pool.");
            return;
        }

        
        proj.transform.position = spawnPos;
        proj.transform.rotation = Quaternion.LookRotation(direction);

        proj.Launch(direction, shootForce, damage, owner, () => projectilePool.Release(proj));
    }

    public void SpawnProjectileBoss(Vector3 spawnPos, Vector3 direction, float shootForce, float damage)
    {
        EnemyProjectile proj = projectilePool.Get();
        if (proj == null)
        {
            Debug.LogWarning("[ProjectileSpawner] No projectiles available in pool.");
            return;
        }


        proj.transform.position = spawnPos;
        proj.transform.rotation = Quaternion.LookRotation(direction);

        proj.LaunchBoss(direction, shootForce, damage, () => projectilePool.Release(proj));
    }

    public EnemyProjectile SpawnIdleProjectile(Vector3 spawnPos, Quaternion rotation)
    {
        EnemyProjectile proj = projectilePool.Get();
        if (proj == null)
        {
            Debug.LogWarning("[ProjectileSpawner] No projectiles available in pool.");
            return null;
        }

        proj.transform.position = spawnPos;
        proj.transform.rotation = rotation;

        // Lo desactivamos físicamente hasta que se dispare
        proj.ResetIdle(() => projectilePool.Release(proj));
        return proj;
    }

}

