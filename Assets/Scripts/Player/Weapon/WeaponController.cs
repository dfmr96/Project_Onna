using System;
using System.Collections;
using Core;
using NaughtyAttributes;
using UnityEngine;

namespace Player.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public static Action<int, int> OnShoot;

        [BoxGroup("Bullet")] 
        [SerializeField] private BulletSettings bulletSetting;
        [BoxGroup("Cooldown")] 
        [SerializeField] private CooldownSettings cooldownSettings;
        [BoxGroup("Ammo")] 
        [SerializeField] private AmmoSettings ammoSettings;

        [BoxGroup("Runtime Debug"), ReadOnly]
        [SerializeField] private int currentAmmo;
        [BoxGroup("Runtime Debug"), ReadOnly]
        [SerializeField] private bool canFire = true;

        private Coroutine _coolingCooldownCoroutine;
        private PlayerModel _playerModel;

        public CooldownSettings Settings => cooldownSettings;

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerInitializedSignal>(OnPlayerReady);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerInitializedSignal>(OnPlayerReady);
        }
        
        private void OnPlayerReady(PlayerInitializedSignal signal)
        {
            _playerModel = signal.Model;

            var stats = _playerModel.StatContext.Source;
            var refs = _playerModel.StatRefs;
            Settings.Init(stats, refs);
            bulletSetting.Init(stats, refs);
            ammoSettings.Init(stats, refs);
            currentAmmo = (int)ammoSettings.MaxAmmo;
        }

        public void Attack()
        {
            if (!canFire || currentAmmo <= 0) return;

            FireBullet();
            currentAmmo--;
            OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);

            if (currentAmmo <= 0)
            {
                StartOverheatCooldown();
            }
            else
            {
                StartCoroutine(FireRateCooldown());
                StartCoolingCooldown();
            }
        }

        private void FireBullet()
        {
            var bullet = Instantiate(bulletSetting.BulletPrefab, bulletSetting.BulletSpawnPoint.position, bulletSetting.BulletSpawnPoint.rotation);
            bullet.Setup(bulletSetting.BulletSpeed,bulletSetting.AttackRange, bulletSetting.Damage);
        }

        private void StartCoolingCooldown()
        {
            if (_coolingCooldownCoroutine == null)
            {
                _coolingCooldownCoroutine = StartCoroutine(CoolingCooldown());
            }
        }

        private void ResetCoolingCooldown()
        {
            if (_coolingCooldownCoroutine != null)
            {
                StopCoroutine(_coolingCooldownCoroutine);
                _coolingCooldownCoroutine = null;
            }
        }

        private void StartOverheatCooldown()
        {
            ResetCoolingCooldown();
            StartCoroutine(OverheatCooldown());
        }

        private IEnumerator FireRateCooldown()
        {
            canFire = false;
            yield return new WaitForSeconds(Settings.FireRate);
            canFire = true;
        }

        private IEnumerator OverheatCooldown()
        {
            canFire = false;
            yield return new WaitForSeconds(Settings.OverheatCooldown);
            currentAmmo = (int)ammoSettings.MaxAmmo;
            canFire = true;
        }

        private IEnumerator CoolingCooldown()
        {
            yield return new WaitForSeconds(Settings.CoolingCooldown);
            currentAmmo = (int)ammoSettings.MaxAmmo;
            _coolingCooldownCoroutine = null;
            OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);
        }
    }
}
