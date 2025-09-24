using System.Collections;
using UnityEngine;

public class DeathScreenTransition : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 1.5f;
    [SerializeField] private float finalScale = 3f;

    private GameObject defeatUIPrefab;

    public void SetDefeatUI(GameObject uiPrefab)
    {
        defeatUIPrefab = uiPrefab;
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(PlayTransition());
    }

    private IEnumerator PlayTransition()
    {
        float t = 0f;
        while (t < scaleDuration)
        {
            t += Time.unscaledDeltaTime; // 👈 importante!
            float progress = Mathf.Clamp01(t / scaleDuration);
            float scale = Mathf.Lerp(0f, finalScale, progress);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        if (defeatUIPrefab != null)
        {
            Instantiate(defeatUIPrefab);
        }
    }

}
