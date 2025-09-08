using Player;
using Player.Weapon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private UIData data;
    [SerializeField] private Image timeCircle;
    //[SerializeField] private Image weaponOverheat;
    //[SerializeField] private Image weaponCooling;

    // --- Balas ---
    [Header("Ammo UI")]
    [SerializeField] private Sprite fullBulletSprite;
    [SerializeField] private Sprite emptyBulletSprite;
    [SerializeField] private GameObject bulletPrefab;      // Prefab de bala (Image)
    [SerializeField] private Transform bulletContainer;    // Contenedor con HorizontalLayoutGroup

    private List<Image> bulletImages = new List<Image>();

    //private CooldownSettings _coolingSettings;
    private float targetCooldownFill;
    private float fillSpeed = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerModel.OnUpdateTime += UpdateTimeUI;
        WeaponController.OnShoot += UpdateBulletsLeft;

        var weapon = PlayerHelper.GetPlayer().GetComponentInChildren<WeaponController>();
        if (weapon != null)
        {
            InitBullets((int)weapon.AmmoSettings.MaxAmmo, weapon.CurrentAmmo);
        }
    }


    private void OnDisable()
    {
        PlayerModel.OnUpdateTime -= UpdateTimeUI;
        WeaponController.OnShoot -= UpdateBulletsLeft;
    }

    private void Update()
    {
        // if (weaponOverheat != null)
        // {
        //     weaponOverheat.fillAmount = Mathf.Lerp(
        //         weaponOverheat.fillAmount,
        //         targetCooldownFill,
        //         Time.deltaTime * fillSpeed
        //     );
        // }
    }

    private void UpdateTimeUI(float timePercent)
    {
        timeCircle.fillAmount = timePercent;
    }

    // -------------------
    // BALAS
    // -------------------

    private void InitBullets(int totalAmmo, int currentAmmo)
    {
        // Limpio lo que hubiera antes
        foreach (Transform child in bulletContainer)
        {
            Destroy(child.gameObject);
        }
        bulletImages.Clear();

        // Creo todas las balas
        for (int i = 0; i < totalAmmo; i++)
        {
            GameObject bulletGO = Instantiate(bulletPrefab, bulletContainer);
            Image bulletImage = bulletGO.GetComponent<Image>();
            bulletImage.sprite = (i < currentAmmo) ? fullBulletSprite : emptyBulletSprite;
            bulletImages.Add(bulletImage);
        }
    }

    private void UpdateBulletsLeft(int actualAmmo, int totalAmmo)
    {
        if (bulletImages.Count != totalAmmo)
        {
            InitBullets(totalAmmo, actualAmmo);
            return;
        }

        for (int i = 0; i < bulletImages.Count; i++)
        {
            var bulletImage = bulletImages[i];
            var animator = bulletImage.GetComponent<Animator>();

            if (i < actualAmmo)
            {
                if (bulletImage.sprite != fullBulletSprite) 
                {
                    bulletImage.sprite = fullBulletSprite;
                    animator?.SetTrigger("ReloadTrigger");
                }
            }
            else
            {
                if (bulletImage.sprite != emptyBulletSprite)
                {
                    bulletImage.sprite = emptyBulletSprite;
                    animator?.SetTrigger("ShootTrigger");
                }
            }
        }
    }

}
