using Player.Stats;
using Player.Stats.Interfaces;
using UnityEngine;

namespace Player.Weapon
{
    [System.Serializable]
    public class BulletSettings
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;

        private IStatSource _stats;
        private StatReferences _statReferences;

        public void Init(IStatSource stats, StatReferences statReferences)
        {
            _statReferences = statReferences;
            _stats = stats;
        }

        public float AttackRange => _stats.Get(_statReferences.attackRange);
        public float Damage => _stats.Get(_statReferences.damage);

        public float BulletSpeed => _stats.Get(_statReferences.bulletSpeed);
        public Bullet BulletPrefab => bulletPrefab;

        public Transform BulletSpawnPoint => bulletSpawnPoint;

    }

    [System.Serializable]
    public class CooldownSettings
    {
        private IStatSource _stats;
        private StatReferences _statReferences;
        public void Init(IStatSource stats, StatReferences statReferences)
        {
            _statReferences = statReferences;
            _stats = stats;
        }

        public float FireRate => _stats.Get(_statReferences.fireRate);
        public float OverheatCooldown => _stats.Get(_statReferences.overheatCooldown);
        public float CoolingCooldown => _stats.Get(_statReferences.coolingCooldown);
    }

    [System.Serializable]
    public class AmmoSettings
    {
        private IStatSource _stats;
        private StatReferences _statReferences;
        public void Init(IStatSource stats, StatReferences statReferences)
        {
            _statReferences = statReferences;
            _stats = stats;
        }
        
        public float MaxAmmo => _stats.Get(_statReferences.maxAmmo); 
    }
}