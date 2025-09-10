using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player.Weapon;
using Player;

public class UI_Ammo : MonoBehaviour
{
    [Header("Ammo UI")]
    [SerializeField] private Sprite fullBulletSprite;
    [SerializeField] private Sprite emptyBulletSprite;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletContainer;

    private List<bool> bulletFilledState = new List<bool>();
    private List<Image> bulletImages = new List<Image>();

    [Header("Sonidos")]
    [SerializeField] private AudioClip reloadBulletSfx;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }


    public IEnumerator InitBulletsDelayed()
    {
        yield return null;
        var weapon = PlayerHelper.GetPlayer()?.GetComponentInChildren<WeaponController>();
        if (weapon != null && weapon.AmmoSettings != null)
        {
            InitBullets((int)weapon.AmmoSettings.MaxAmmo, weapon.CurrentAmmo);
        }
    }

    private void InitBullets(int totalAmmo, int currentAmmo)
    {
        foreach (Transform child in bulletContainer)
            Destroy(child.gameObject);

        bulletImages.Clear();
        bulletFilledState.Clear();

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

    public void UpdateBulletsLeft(int actualAmmo, int totalAmmo)
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

            if (bulletFilledState[i] != shouldBeFull)
            {
                if (shouldBeFull)
                {
                    bulletImage.sprite = fullBulletSprite;
                    animator?.SetTrigger("ReloadTrigger");

                    // reproducir sonido de recarga de bala
                    if(reloadBulletSfx != null)
                        audioSource.PlayOneShot(reloadBulletSfx);
                }
                else
                {
                    bulletImage.sprite = emptyBulletSprite;
                    animator?.SetTrigger("ShootTrigger");
                }

                bulletFilledState[i] = shouldBeFull;
            }
            if (i == actualAmmo - 1 && shouldBeFull)
            {
                bulletImage.color = new Color(1.5f, 1.5f, 1.5f);
            }
            else
            {
                bulletImage.color = Color.white;
            }
        }
    }


    public IEnumerator BlinkEmptyBullets(int times = 6, float interval = 0.1f)
    {
        for (int i = 0; i < times; i++)
        {
            foreach (var img in bulletImages)
            {
                if (img.sprite == emptyBulletSprite)
                {
                    img.enabled = !img.enabled; // toggle on/off
                }
            }
            yield return new WaitForSeconds(interval);
        }

        // aseguramos que queden todos visibles al terminar
        foreach (var img in bulletImages)
        {
            if (img.sprite == emptyBulletSprite)
                img.enabled = true;
        }
    }

}
