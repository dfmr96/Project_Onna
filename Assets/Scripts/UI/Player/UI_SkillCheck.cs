using System.Collections;
using UnityEngine;

namespace Player.Weapon
{
    public class UI_SkillCheck : MonoBehaviour
    {
        [SerializeField] private RectTransform movingBar;
        [SerializeField] private RectTransform targetZone;
        [SerializeField] private RectTransform barContainer;
        [SerializeField] private GameObject skillCheckGO; 

        private bool active = false;
        private WeaponController weaponController;

        void Start()
        {
            Hide(); // Ocultar al inicio
        }

        public void Show()
        {
            skillCheckGO.SetActive(true); // Mostrar UI
            active = true;
            weaponController = FindObjectOfType<WeaponController>();
            ResetBar();
        }

        public void Hide()
        {
            skillCheckGO.SetActive(false); // Ocultar UI
            active = false;
        }

        public void ResetBar()
        {
            // Posicionar la barra al inicio del contenedor
            movingBar.localPosition = new Vector3(-barContainer.rect.width / 2f, movingBar.localPosition.y, 0);
        }

        public void UpdateMovingBar(float progress)
        {
            // Mover la barra de izquierda a derecha dentro del contenedor
            float startX = -barContainer.rect.width / 2f;
            float endX = barContainer.rect.width / 2f;

            float newX = Mathf.Lerp(startX, endX, progress);
            movingBar.localPosition = new Vector3(newX, movingBar.localPosition.y, 0);
        }

        public bool IsOverTarget()
        {
            // Comprobamos si el centro de la barra está dentro del target
            Vector3 barCenter = movingBar.position;
            Vector3 targetPos = targetZone.position;

            float halfWidth = targetZone.rect.width / 2f;
            float halfHeight = targetZone.rect.height / 2f;

            return barCenter.x >= targetPos.x - halfWidth &&
                   barCenter.x <= targetPos.x + halfWidth &&
                   barCenter.y >= targetPos.y - halfHeight &&
                   barCenter.y <= targetPos.y + halfHeight;
        }

        // Llamar desde WeaponController al presionar R
        public void TrySkillCheck()
        {
            if (!active || weaponController == null) return;

            if (IsOverTarget())
            {
                // Éxito: recarga rápida
                weaponController.SetSkillCheckSuccess(true);
            }
            else
            {
                // Fallo: recarga lenta
                weaponController.SetSkillCheckSuccess(false);
            }

            Hide(); // Ocultar UI
        }

        public bool IsActive() => active;
    }
}
