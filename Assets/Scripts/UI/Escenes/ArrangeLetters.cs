using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrangeLetters : MonoBehaviour
{
    [Header("Letras en orden")]
    [SerializeField] private List<Image> letters;  // arrastrar aquí los Image de las letras

    [Header("Animación")]
    [SerializeField] private float fillDuration = 0.3f;   // duración de llenado por letra
    [SerializeField] private float delayBetween = 0.05f;  // delay entre cada letra

    public void ShowAndFillLetters()
    {
        StopAllCoroutines();
        StartCoroutine(FillLettersRoutine());
    }

    private IEnumerator FillLettersRoutine()
    {
        foreach (Image letter in letters)
        {
            if (letter == null) continue;

            letter.fillAmount = 0f;     // resetear fill
            letter.gameObject.SetActive(true);

            yield return StartCoroutine(FillLetter(letter));

            yield return new WaitForSecondsRealtime(delayBetween);
        }
    }

    private IEnumerator FillLetter(Image letter)
    {
        float t = 0f;
        while (t < fillDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / fillDuration);
            letter.fillAmount = progress;
            yield return null;
        }
        letter.fillAmount = 1f;
    }
}
