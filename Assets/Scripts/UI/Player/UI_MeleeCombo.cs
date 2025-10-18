using UnityEngine;
using UnityEngine.UI;
using Player.Melee;
using Player;
using System.Collections;

public class UI_MeleeCombo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MeleeController meleeController;
    [SerializeField] private Image firstHitIcon;
    [SerializeField] private Image secondHitIcon;
    [SerializeField] private Animator firstHitAnimator;
    [SerializeField] private Animator secondHitAnimator;

    private bool firstHitDone = false;
    private bool secondHitDone = false;

    private bool firstReadyToActivate = false;
    private bool secondReadyToActivate = false; 

    private void Start()
    {
        if (!meleeController)
        {
            var player = PlayerHelper.GetPlayer();
            if (player != null)
                meleeController = player.GetComponentInChildren<MeleeController>();
        }

        // Suscribirse al evento de cooldown
        if (meleeController != null)
            meleeController.OnCooldownComplete += HandleCooldownComplete;

        // Inicializamos animaciones en Idle
        SetIdleState();
    }

    private void OnDestroy()
    {
        if (meleeController != null)
            meleeController.OnCooldownComplete -= HandleCooldownComplete;
    }

    private void Update()
    {
        if (meleeController == null) return;


        if (meleeController.ComboStep >= 1 && !firstHitDone)
        {
            TriggerHit(firstHitAnimator, firstHitIcon.rectTransform);
            firstHitDone = true;
            firstReadyToActivate = true;
        }

        // Segundo golpe
        if (meleeController.ComboStep >= 2 && !secondHitDone)
        {
            TriggerHit(secondHitAnimator, secondHitIcon.rectTransform);
            secondHitDone = true;
            secondReadyToActivate = true;
        }

    }

    private void TriggerHit(Animator animator, RectTransform rect)
    {
        if (animator != null)
            animator.SetTrigger("HitTriggered");

        if (rect != null)
            StartCoroutine(Shake(rect, 0.15f, 5f)); // duración 0.15s, intensidad 5px
    }

    private IEnumerator Shake(RectTransform rect, float duration, float magnitude)
    {
        Vector3 originalPos = rect.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            rect.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.localPosition = originalPos;
    }


    private void HandleCooldownComplete()
    {
        // Llamamos ActivateIcons automáticamente cuando termina el cooldown
        ActivateIcons();
    }

    private void ActivateIcons()
    {
        firstHitDone = false;
        secondHitDone = false;

        if (firstHitAnimator != null)
        {
            firstHitAnimator.SetTrigger("ActivateTriggered"); // Idle_Destroy → Spawn → Idle
        }

        if (secondHitAnimator != null)
        {
            secondHitAnimator.SetTrigger("ActivateTriggered");
        }
    }

    private void SetIdleState()
    {
        if (firstHitAnimator != null) firstHitAnimator.Play("Idle", 0, 0f);
        if (secondHitAnimator != null) secondHitAnimator.Play("Idle", 0, 0f);
    }
}
