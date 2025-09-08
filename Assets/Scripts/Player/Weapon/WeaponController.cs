using System;
using System.Collections;
using Core;
using NaughtyAttributes;
using UnityEngine;

namespace Player.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        public static event Action<int, int> OnShoot;
        //public static event Action<float, float> OnCooling;
        public static event Action<float, float> OnReloading;


        [BoxGroup("Bullet")] 
        [SerializeField] private BulletSettings bulletSetting;
        //[BoxGroup("Cooldown")] 
        //[SerializeField] private CooldownSettings cooldownSettings;
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
        private LineRenderer lineRenderer;

        [BoxGroup("Sounds")]
        [SerializeField] private AudioClip shootFx;
        [BoxGroup("Sounds")]
        [SerializeField] private AudioClip overheathFx;
        private AudioSource audioSource;


        //private Coroutine _coolingCooldownCoroutine;
        private PlayerModel _playerModel;

        //public CooldownSettings Settings => cooldownSettings;


        //Hardcodeado: Cambiar
        private float fireRate = 0.2f;
        private float reloadTime = 1f;

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerInitializedSignal>(OnPlayerReady);
        }
        
        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerInitializedSignal>(OnPlayerReady);
        }

        private void Start()
        {
            InitializeVisuals();
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

            //if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < ammoSettings.MaxAmmo)
            //{
            //    StartCoroutine(Reload());
            //}
        }

        private void OnPlayerReady(PlayerInitializedSignal signal)
        {
            _playerModel = signal.Model;

            var stats = _playerModel.StatContext.Source;
            var refs = _playerModel.StatRefs;
            //Settings.Init(stats, refs);
            bulletSetting.Init(stats, refs);
            ammoSettings.Init(stats, refs);
            currentAmmo = (int)ammoSettings.MaxAmmo;
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
            //if (!canFire || currentAmmo <= 0) return;
            if (!canFire || isReloading || currentAmmo <= 0) return;


            FireBullet();
            currentAmmo--;
            OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);

            if (currentAmmo <= 0)
            {
                //StartOverheatCooldown();
                StartCoroutine(Reload());

            }
            else
            {
                StartCoroutine(FireRateCooldown());
                //StartCoolingCooldown();
            }
        }

        private void FireBullet()
        {
            if (muzzleFlashInstance != null)
            {
                muzzleFlashInstance.transform.position = bulletSetting.BulletSpawnPoint.position;
                muzzleFlashInstance.transform.rotation = bulletSetting.BulletSpawnPoint.rotation;
                muzzleFlashInstance.Play();
            }

            var bullet = Instantiate(bulletSetting.BulletPrefab, bulletSetting.BulletSpawnPoint.position, bulletSetting.BulletSpawnPoint.rotation);
            bullet.Setup(bulletSetting.BulletSpeed,bulletSetting.AttackRange, bulletSetting.Damage);
            audioSource.PlayOneShot(shootFx);
        }

        //private void StartCoolingCooldown()
        //{
        //    if (_coolingCooldownCoroutine == null)
        //    {
        //        Debug.Log("Cooling cooldown Called");
        //    }
        //    else
        //    {
        //        StopCoroutine(_coolingCooldownCoroutine);
        //        Debug.Log("Cooling cooldown restarted");
        //    }
        //    _coolingCooldownCoroutine = StartCoroutine(CoolingCooldown());
        //}

        //private void StopCoolingCooldown()
        //{
        //    if (_coolingCooldownCoroutine != null)
        //    {
        //        StopCoroutine(_coolingCooldownCoroutine);
                
        //        Debug.Log("Cooling cooldown stopped");
        //        _coolingCooldownCoroutine = null;
        //    }
        //}

        //private void StartOverheatCooldown()
        //{
        //    StopCoolingCooldown();
        //    StartCoroutine(OverheatCooldown());
        //    audioSource?.PlayOneShot(overheathFx);
        //}

        private IEnumerator FireRateCooldown()
        {
            canFire = false;
            //yield return new WaitForSeconds(Settings.FireRate);
            yield return new WaitForSeconds(fireRate);

            canFire = true;
        }

       private IEnumerator Reload()
        {
            isReloading = true;
            canFire = false;
            audioSource?.PlayOneShot(overheathFx);

            float bulletReloadTime = 0.15f;

            while (currentAmmo < ammoSettings.MaxAmmo)
            {
                yield return new WaitForSeconds(bulletReloadTime);

                currentAmmo++;
                OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo); 
                // acá la UI recibe el update y puede animar esa bala
            }

            isReloading = false;
            canFire = true;
        }


        //private IEnumerator OverheatCooldown()
        //{
        //    Debug.Log("Overheat cooldown Called");
        //    OnCooling?.Invoke(Settings.CoolingCooldown, Settings.CoolingCooldown);
        //    Debug.Log("OnCooling event called");
        //    canFire = false;
        //    yield return new WaitForSeconds(Settings.OverheatCooldown);
        //    currentAmmo = (int)ammoSettings.MaxAmmo;

        //    OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);
        //    canFire = true;
        //    Debug.Log("Overheat cooldown Finished");
        //}

        //private IEnumerator CoolingCooldown()
        //{
        //    float coolingTimer = 0f;

        //    while (coolingTimer < Settings.CoolingCooldown)
        //    {
        //        coolingTimer += Time.deltaTime;
        //        OnCooling?.Invoke(coolingTimer, Settings.CoolingCooldown);
        //        yield return null;
        //    }
        //    currentAmmo = (int)ammoSettings.MaxAmmo;
        //    _coolingCooldownCoroutine = null;
        //    Debug.Log("Cooling cooldown finished");
        //    OnShoot?.Invoke(currentAmmo, (int)ammoSettings.MaxAmmo);
        //}

        private void LaserEffect()
        {
            Vector3 direction = laserOrigin.forward;
            Vector3 endPos = laserOrigin.position + direction * laserLength;

            Ray ray = new Ray(laserOrigin.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, laserLength))
            {
                endPos = hit.point;
            }

            lineRenderer.SetPosition(0, laserOrigin.position);
            lineRenderer.SetPosition(1, endPos);

            //particulas al final del rayo
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



        //Prueba visual para balubis

        //private void OnGUI()
        //{
        //    int w = Screen.width;
        //    int h = Screen.height;

        //    string bars = new string('|', currentAmmo);

        //    int rectWidth = 400;
        //    int rectHeight = 30;

        //    Rect rect = new Rect(
        //        (w - rectWidth) / 2,
        //        h - rectHeight - 20,
        //        rectWidth,
        //        rectHeight
        //    );

        //    GUI.Label(rect, $"Ammo: {bars}");
        //}
    }


}

