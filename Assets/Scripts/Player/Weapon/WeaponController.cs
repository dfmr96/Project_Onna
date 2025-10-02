using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using NaughtyAttributes;
using UnityEngine;

namespace Player.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public static event Action<int, int> OnShoot;
        //public static event Action<float, float> OnReloading;

        [BoxGroup("Bullet")]
        [SerializeField] private BulletSettings bulletSetting;

        [BoxGroup("Ammo")]
        [SerializeField] private AmmoSettings ammoSettings;
        public AmmoSettings AmmoSettings => ammoSettings;

        [BoxGroup("Runtime Debug"), ReadOnly]
        [SerializeField] private int currentAmmo;
        public int CurrentAmmo => currentAmmo;

        [BoxGroup("Runtime Debug"), ReadOnly]
        [SerializeField] private bool canFire = true;
        [BoxGroup("Runtime Debug"), ReadOnly]
        [SerializeField] private bool isReloading = false;

        [BoxGroup("Laser Effect")]
        [SerializeField] private ParticleSystem laserGunParticlesPrefab;
        [SerializeField] private ParticleSystem shootGunParticlesPrefab;
        [SerializeField] private Transform laserOrigin;

        private float laserLength;
        private ParticleSystem impactParticlesInstance;
        private ParticleSystem muzzleFlashInstance;
        [SerializeField] LineRenderer lineRenderer;

        private bool isSkillCheckActive = false;
        private bool skillCheckSuccess = false;
        private UI_SkillCheck reloadUI;

        [BoxGroup("Sounds")]
        [SerializeField] private AudioClip shootFx;
        [BoxGroup("Sounds")]
        [SerializeField] private AudioClip overheathFx;
        private AudioSource audioSource;

        [BoxGroup("Runtime Debug"), ReadOnly]
        [SerializeField] private bool nextShotDoubleDamage = false;

        private PlayerModel _playerModel;

        // Hardcodeado
        private float fireRate = 0.15f;

        private List<BulletModifierSO> activeBulletModifiers = new List<BulletModifierSO>();
        private PlayerControllerEffect _playerEffect;


        private void OnEnable()
        {
            EventBus.Subscribe<PlayerInitializedSignal>(OnPlayerReady);

            var inputHandler = FindObjectOfType<PlayerInputHandler>();
            if (inputHandler != null)
                inputHandler.ReloadPerformed += OnReloadInput;

            var playerEffect = FindObjectOfType<PlayerControllerEffect>();
            if (playerEffect != null)
                _playerEffect = playerEffect;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerInitializedSignal>(OnPlayerReady);

            var inputHandler = FindObjectOfType<PlayerInputHandler>();
            if (inputHandler != null)
                inputHandler.ReloadPerformed -= OnReloadInput;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void InitializeVisuals()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>(true);
            laserLength = bulletSetting.AttackRange;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            if (laserGunParticlesPrefab != null)
            {
                impactParticlesInstance = Instantiate(laserGunParticlesPrefab);
                impactParticlesInstance.transform.SetParent(null);
                impactParticlesInstance.Stop();
            }

            if (shootGunParticlesPrefab != null)
            {
                muzzleFlashInstance = Instantiate(shootGunParticlesPrefab, bulletSetting.BulletSpawnPoint.position, bulletSetting.BulletSpawnPoint.rotation);
                muzzleFlashInstance.transform.SetParent(null);
                muzzleFlashInstance.Stop();
            }
        }

        private void Update()
        {
            LaserEffect();
        }

        private void OnPlayerReady(PlayerInitializedSignal signal)
        {
            _playerModel = signal.Model;
            _playerEffect = signal.PlayerEffect;

            var stats = _playerModel.StatContext.Source;
            var refs = _playerModel.StatRefs;

            bulletSetting.Init(stats, refs);
            ammoSettings.Init(stats, refs);
            currentAmmo = (int)ammoSettings.MaxAmmo;

            InitializeVisuals();
        }

        private void OnReloadInput()
        {
            if (isSkillCheckActive)
            {
                TrySkillCheck();
            }
            else
            {
                Reloading();
            }
        }

        public void Reloading()
        {
            if (!isReloading && currentAmmo != ammoSettings.MaxAmmo)
            {
                StartCoroutine(Reload());
            }
        }

        public void Attack()
        {
            if (!canFire || isReloading || currentAmmo <= 0) return;

            FireBullet();
            currentAmmo--;
            OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
            else
            {
                StartCoroutine(FireRateCooldown());
            }
        }

        public void SetActiveBulletModifiers(List<BulletModifierSO> modifiers)
        {
            activeBulletModifiers = modifiers;
        }

     

        private void FireBullet()
        {
            if (muzzleFlashInstance != null)
            {
                muzzleFlashInstance.transform.position = bulletSetting.BulletSpawnPoint.position;
                muzzleFlashInstance.transform.rotation = bulletSetting.BulletSpawnPoint.rotation;
                muzzleFlashInstance.Play();
            }

            float damage = bulletSetting.Damage;
            if (nextShotDoubleDamage)
            {
                damage *= 2f;
                nextShotDoubleDamage = false;
            }

            //var bullet = Instantiate(bulletSetting.BulletPrefab, bulletSetting.BulletSpawnPoint.position, bulletSetting.BulletSpawnPoint.rotation);
            //bullet.Setup(bulletSetting.BulletSpeed, bulletSetting.AttackRange, damage);

            var bulletObj = Instantiate(bulletSetting.BulletPrefab, bulletSetting.BulletSpawnPoint.position, bulletSetting.BulletSpawnPoint.rotation);
            bulletObj.Setup(bulletSetting.BulletSpeed, bulletSetting.AttackRange, bulletSetting.Damage);

            // Registrarle todos los modificadores activos
            var activeModifiers = _playerEffect.GetActiveBulletModifiers();
            foreach (var mod in activeModifiers)
            {
                bulletObj.RegisterModifier(mod, _playerEffect);
            }

            // Aplicar materiales después de registrar todos
            bulletObj.ApplyTrailMaterials();

            audioSource.PlayOneShot(shootFx);
        }




        private IEnumerator FireRateCooldown()
        {
            canFire = false;
            yield return new WaitForSeconds(fireRate);
            canFire = true;
        }

        private IEnumerator Reload()
        {
            bool isEmptyReload = currentAmmo == 0;
            float bulletReloadTime = 0.15f;

            isReloading = true;
            canFire = false;
            audioSource?.PlayOneShot(overheathFx);

            if (isEmptyReload)
            {
                var ammoUI = FindObjectOfType<UI_Ammo>();
                if (ammoUI != null)
                    yield return ammoUI.StartCoroutine(ammoUI.BlinkEmptyBullets());

                yield return StartCoroutine(EmptyReloadMinigame());
            }
            else
            {
                while (currentAmmo < ammoSettings.MaxAmmo)
                {
                    yield return new WaitForSeconds(bulletReloadTime);
                    currentAmmo++;
                    OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);
                }
            }

            isReloading = false;
            canFire = true;
        }

        private IEnumerator EmptyReloadMinigame()
        {
            reloadUI = FindObjectOfType<UI_SkillCheck>();
            if (reloadUI == null) yield break;

            isSkillCheckActive = true;
            reloadUI.Show(); // dispara la animación y la barra
            while (isSkillCheckActive)
            {
                yield return null;
            }

            // aplicar efecto según skillCheckSuccess
            if (skillCheckSuccess)
            {
                nextShotDoubleDamage = true;
                currentAmmo = (int)ammoSettings.MaxAmmo;
                OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);
            }
            else
            {
                // recarga lenta
                float bulletReloadTime = 0.15f * 2f;
                while (currentAmmo < ammoSettings.MaxAmmo)
                {
                    yield return new WaitForSeconds(bulletReloadTime);
                    currentAmmo++;
                    OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);
                }
            }
        }


        public void TrySkillCheck()
        {
            if (!isSkillCheckActive || reloadUI == null) return;
            reloadUI.TrySkillCheck();
        }


        private void LaserEffect()
        {
            Vector3 direction = laserOrigin.forward;
            Vector3 endPos = laserOrigin.position + direction * laserLength;

            if (Physics.Raycast(laserOrigin.position, direction, out RaycastHit hit, laserLength))
                endPos = hit.point;

            lineRenderer.SetPosition(0, laserOrigin.position);
            lineRenderer.SetPosition(1, endPos);

            if (impactParticlesInstance != null)
            {
                impactParticlesInstance.transform.position = endPos;
                Vector3 toLaserOrigin = (laserOrigin.position - endPos).normalized;
                if (toLaserOrigin != Vector3.zero)
                    impactParticlesInstance.transform.rotation = Quaternion.LookRotation(toLaserOrigin);

                if (!impactParticlesInstance.isPlaying)
                    impactParticlesInstance.Play();
            }
        }

        public void SetSkillCheckSuccess(bool success)
        {
            skillCheckSuccess = success;
        }

        public void NotifySkillCheckEnded()
        {
            isSkillCheckActive = false;
        }
    }
}
