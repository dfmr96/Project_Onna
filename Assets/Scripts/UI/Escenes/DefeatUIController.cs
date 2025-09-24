using UnityEngine;
using System.Collections;

public class DefeatUIController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ArrangeLetters arrangeLetters; // referencia al otro script
    [Header("Shake Effect")]
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float shakeMagnitude = 5f; // intensidad del temblor en píxeles

    private RectTransform rectTransform;

    private void Awake()
    {
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShakeUI());
    }

    public void OnIntroAnimationFinished()
    {
        if (arrangeLetters != null)
        {
            arrangeLetters.ShowAndFillLetters();
        }
    }

    public void OnReturnToHubButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToHub();
        }
        else
        {
            Debug.LogError("GameManager no encontrado en la escena.");
        }
    }

    private IEnumerator ShakeUI()
    {
        if (rectTransform == null) yield break;

        Vector3 originalPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

        rectTransform.anchoredPosition = originalPos; // volver a posición original
    }
}
