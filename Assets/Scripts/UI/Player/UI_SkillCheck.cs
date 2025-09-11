using System.Collections;
using UnityEngine;

namespace Player.Weapon
{
    public class UI_SkillCheck : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private RectTransform movingBar;
        [SerializeField] private RectTransform targetZone;
        [SerializeField] private RectTransform barContainer;
        [SerializeField] private GameObject skillCheckGO;

        [Header("Animador")]
        [SerializeField] private Animator barAnimator;
        [SerializeField] private Animator moveBarAnimator;// Animator con las animaciones: Open, Success, Fail

        private bool active = false;
        private WeaponController weaponController;
        private bool skillCheckSuccess = false;
        private Coroutine moveBarCoroutine;

        private float duration = 2f; // duración del skillcheck

        [Header("Sonidos")]
        [SerializeField] private AudioClip successSfx;
        [SerializeField] private AudioClip failSfx;

        private AudioSource audioSource;

        void Start()
        {
            HideImmediate();
                audioSource = GetComponent<AudioSource>();
                if(audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void Show()
        {
            skillCheckGO.SetActive(true);
            active = true;
            weaponController = FindObjectOfType<WeaponController>();

            movingBar.gameObject.SetActive(false);
            targetZone.gameObject.SetActive(false);

            ResetBar();
            skillCheckSuccess = false;

            barAnimator.SetTrigger("Open");

            // Iniciar movimiento de la barra
        }

        private IEnumerator MoveBarCoroutine()
        {
            float elapsed = 0f;
            skillCheckSuccess = false;

            while (elapsed < duration && !skillCheckSuccess)
            {
                elapsed += Time.deltaTime;
                UpdateMovingBar(elapsed / duration);
                yield return null;
            }

            if (!skillCheckSuccess)
            {
                FailSkillCheck();
            }
        }

        private void FailSkillCheck()
        {
            skillCheckSuccess = false;
            weaponController.SetSkillCheckSuccess(false);

            movingBar.gameObject.SetActive(true);
            moveBarAnimator.SetTrigger("BarFeedback");
            barAnimator.SetTrigger("Fail");

            // Reproducir sonido de fallo
            if(failSfx != null)
                audioSource.PlayOneShot(failSfx);
        }

        public void ResetBar()
        {
            movingBar.localPosition = new Vector3(-barContainer.rect.width / 2f, movingBar.localPosition.y, 0);
        }

        public void UpdateMovingBar(float progress)
        {
            float startX = -barContainer.rect.width / 2f;
            float endX = barContainer.rect.width / 2f;

            float newX = Mathf.Lerp(startX, endX, progress);
            movingBar.localPosition = new Vector3(newX, movingBar.localPosition.y, 0);
        }

        public bool IsOverTarget()
        {
            Vector2 localPoint = barContainer.InverseTransformPoint(movingBar.position);

            Rect targetRect = new Rect(
                targetZone.localPosition.x - targetZone.rect.width / 2f,
                targetZone.localPosition.y - targetZone.rect.height / 2f,
                targetZone.rect.width,
                targetZone.rect.height
            );

            return targetRect.Contains(localPoint);
        }

        public void TrySkillCheck()
        {
            if (!active || weaponController == null) return;

            if (moveBarCoroutine != null)
                StopCoroutine(moveBarCoroutine);

            targetZone.gameObject.SetActive(false);
            movingBar.gameObject.SetActive(true);
            moveBarAnimator.SetTrigger("BarFeedback");

            if (IsOverTarget())
            {
                skillCheckSuccess = true;
                weaponController.SetSkillCheckSuccess(true);
                barAnimator.SetTrigger("Success");

                // Reproducir sonido de éxito
                if(successSfx != null)
                    audioSource.PlayOneShot(successSfx);
            }
            else
            {
                FailSkillCheck();
            }
        }

        public bool IsActive() => active;

        // Llamar desde el último frame de la animación "Open"
        public void OnOpenAnimationEnd()
        {
            movingBar.gameObject.SetActive(true);
            targetZone.gameObject.SetActive(true);
            moveBarCoroutine = StartCoroutine(MoveBarCoroutine());
        }

        // Llamar desde el último frame de las animaciones "Success" y "Fail"
        public void OnResultAnimationEnd()
        {
            Hide(); // oculta HUD

            if (weaponController != null)
            {
                // Le avisamos que terminó y si fue éxito o fallo
                weaponController.SetSkillCheckSuccess(skillCheckSuccess);
                weaponController.NotifySkillCheckEnded();
            }
        }

        public void Hide()
        {
            skillCheckGO.SetActive(false);
            active = false;
        }


        public void HideMoveBar()
        {
            movingBar.gameObject.SetActive(false);
        }

        private void HideImmediate()
        {
            skillCheckGO.SetActive(false);
            movingBar.gameObject.SetActive(false);
            targetZone.gameObject.SetActive(false);
            active = false;
        }
    }
}
