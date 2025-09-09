using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmo : MonoBehaviour
{
    [Header("Sprites de balas")]
        [SerializeField] private Sprite fullBulletSprite;
        [SerializeField] private Sprite emptyBulletSprite;

        [Header("Referencias UI")]
        [SerializeField] private GameObject bulletPrefab; // Prefab con un Image
        [SerializeField] private Transform bulletContainer; // Panel con HorizontalLayoutGroup

        private List<Image> bulletImages = new List<Image>();

        /// <summary>
        /// Inicializa la UI de balas
        /// </summary>
        public void Init(int totalAmmo)
        {
            // Limpio por si ya había algo
            foreach (Transform child in bulletContainer)
            {
                Destroy(child.gameObject);
            }
            bulletImages.Clear();

            // Creo las balas en la UI
            for (int i = 0; i < totalAmmo; i++)
            {
                GameObject bulletGO = Instantiate(bulletPrefab, bulletContainer);
                Image bulletImage = bulletGO.GetComponent<Image>();
                bulletImage.sprite = fullBulletSprite;
                bulletImages.Add(bulletImage);
            }
        }

        /// <summary>
        /// Actualiza la UI según balas actuales
        /// </summary>
        public void UpdateAmmo(int currentAmmo)
        {
            for (int i = 0; i < bulletImages.Count; i++)
            {
                bulletImages[i].sprite = (i < currentAmmo) ? fullBulletSprite : emptyBulletSprite;
            }
        }
}
