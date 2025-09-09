using Player;
using Player.Weapon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("General UI")]
    [SerializeField] private UIData data;
    [SerializeField] private Image timeCircle;
    // [SerializeField] private Image weaponOverheat;
    // [SerializeField] private Image weaponCooling;

    [Header("Ammo UI")]
    [SerializeField] private Sprite fullBulletSprite;
    [SerializeField] private Sprite emptyBulletSprite;
    [SerializeField] private GameObject bulletPrefab;      // Prefab de bala (Image)
    [SerializeField] private Transform bulletContainer;    // Contenedor con HorizontalLayoutGroup

    private List<bool> bulletFilledState = new List<bool>(); // true = llena, false = vacía
    private List<Image> bulletImages = new List<Image>();

    private float fillSpeed = 2f;
    private float targetCooldownFill;

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
        // Aquí podrías animar weaponOverheat o weaponCooling si lo quieres
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
        // Limpiar contenedor
        foreach (Transform child in bulletContainer)
            Destroy(child.gameObject);

        bulletImages.Clear();
        bulletFilledState.Clear();

        // Crear balas
        for (int i = 0; i < totalAmmo; i++)
        {
            GameObject bulletGO = Instantiate(bulletPrefab, bulletContainer);
            Image bulletImage = bulletGO.GetComponent<Image>();
            bulletImages.Add(bulletImage);

            bool isFull = i < currentAmmo;
            bulletImage.sprite = isFull ? fullBulletSprite : emptyBulletSprite;
            bulletFilledState.Add(isFull);
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

            bool shouldBeFull = i < actualAmmo;

            // -------------------------------
            // Animación solo si cambió de estado
            // -------------------------------
            if (bulletFilledState[i] != shouldBeFull)
            {
                if (shouldBeFull)
                {
                    bulletImage.sprite = fullBulletSprite;
                    animator?.SetTrigger("ReloadTrigger");
                }
                else
                {
                    bulletImage.sprite = emptyBulletSprite;
                    animator?.SetTrigger("ShootTrigger");
                }

                bulletFilledState[i] = shouldBeFull;
            }

            // -------------------------------
            // Resaltar la última bala llena
            // -------------------------------
            if (i == actualAmmo - 1 && shouldBeFull)
            {
                bulletImage.color = new Color(0.7f, 0.7f, 0.7f); // gris oscuro
            }
            else
            {
                bulletImage.color = Color.white; // color normal
            }
        }
    }
}
