using UnityEngine;
using TMPro;
using System.Collections;

public class DefeatUIController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ArrangeLetters arrangeLetters; // referencia al otro script
    [Header("Shake Effect")]
    [SerializeField] private float shakeDuration = 10f;
    [SerializeField] private float shakeMagnitude = 2f; // intensidad del temblor en píxeles

    [SerializeField] private TextMeshProUGUI buttonText;

    public RectTransform rectTransform;

    private void Awake()
    {
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    private void OnEnable() => StartCoroutine(ShakeUI());

    public void OnIntroAnimationFinished()
    {
        if (arrangeLetters != null)
        {
            arrangeLetters.ShowAndFillLetters();
        }
    }

    public void OnReturnToHubButton() => GameManager.Instance?.ReturnToHub();

    public void OnReturnToHubTutorialButton() => GameManager.Instance?.ReturnToHubTutorial();
    
    public void OnReturnToTutorialButton() => GameManager.Instance?.ReturnToTutorial();

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
        buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1);
    }
}
