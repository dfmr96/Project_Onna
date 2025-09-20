using UnityEngine;
using System.Collections;

public class UI_MainMenu_ParallaxZoom : MonoBehaviour
{
    public static UI_MainMenu_ParallaxZoom Instance { get; private set; }

    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform layer;
        public float zoomScale = 1.2f;
        public Vector2 zoomDirection;
        public float zoomOffset = 200f;
    }

    [Header("Parallax Layers")]
    public ParallaxLayer[] layers;

    [Header("Ojo (UI)")]
    public RectTransform eye;
    public float eyeScaleFactor = 1.1f;
    public float eyeMoveOffset = 20f;

    public float transitionTime = 1f;
    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void NextLayer()
    {
        if (currentIndex < layers.Length - 1)
        {
            StartCoroutine(ZoomTransition(currentIndex, currentIndex + 1, true));
            currentIndex++;
        }
    }

    public void PreviousLayer()
    {
        if (currentIndex > 0)
        {
            StartCoroutine(ZoomTransition(currentIndex, currentIndex - 1, false));
            currentIndex--;
        }
    }
    
    public IEnumerator ZoomToNextCoroutine()
    {
        if (currentIndex < layers.Length - 1)
        {
            int nextIndex = currentIndex + 1;
            yield return StartCoroutine(ZoomTransition(currentIndex, nextIndex, true));
            currentIndex = nextIndex; // actualizar al final
        }
    }

    public IEnumerator ZoomToPreviousCoroutine()
    {
        if (currentIndex > 0)
        {
            int prevIndex = currentIndex - 1;
            yield return StartCoroutine(ZoomTransition(currentIndex, prevIndex, false));
            currentIndex = prevIndex;
        }
    }

    private IEnumerator ZoomTransition(int from, int to, bool forward)
    {
        var fromLayer = layers[from];
        var toLayer = layers[to];

        // --- Parallax ---
        Vector2 fromStartPos = fromLayer.layer.anchoredPosition;
        Vector2 toStartPos = toLayer.layer.anchoredPosition;

        float fromStartScale = fromLayer.layer.localScale.x;
        float toStartScale = toLayer.layer.localScale.x;

        Vector2 fromDir = fromLayer.zoomDirection.normalized * fromLayer.zoomOffset;
        Vector2 toDir = toLayer.zoomDirection.normalized * toLayer.zoomOffset;

        Vector2 fromEndPos = forward ? fromStartPos + fromDir : fromStartPos - fromDir;
        Vector2 toEndPos = forward ? toStartPos + toDir : toStartPos - toDir;

        float fromEndScale = forward ? fromStartScale * fromLayer.zoomScale : fromStartScale / fromLayer.zoomScale;
        float toEndScale = forward ? toStartScale * toLayer.zoomScale : toStartScale / toLayer.zoomScale;

        // --- Ojo ---
        Vector2 eyeStartPos = eye.anchoredPosition;
        float eyeStartScale = eye.localScale.x;

        Vector2 eyeEndPos;
        float eyeEndScale;

        if (forward)
        {
            eyeEndPos = eyeStartPos - (toDir.normalized * eyeMoveOffset);
            eyeEndScale = eyeStartScale * eyeScaleFactor;
        }
        else
        {
            eyeEndPos = eyeStartPos + (fromDir.normalized * eyeMoveOffset);
            eyeEndScale = eyeStartScale / eyeScaleFactor;
        }

        float elapsed = 0f;


        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionTime;

            // Parallax
            fromLayer.layer.anchoredPosition = Vector2.Lerp(fromStartPos, fromEndPos, t);
            toLayer.layer.anchoredPosition = Vector2.Lerp(toStartPos, toEndPos, t);

            float fromScale = Mathf.Lerp(fromStartScale, fromEndScale, t);
            float toScale = Mathf.Lerp(toStartScale, toEndScale, t);

            fromLayer.layer.localScale = Vector3.one * fromScale;
            toLayer.layer.localScale = Vector3.one * toScale;

            eye.anchoredPosition = Vector2.Lerp(eyeStartPos, eyeEndPos, t);
            float eyeScale = Mathf.Lerp(eyeStartScale, eyeEndScale, t);
            eye.localScale = Vector3.one * eyeScale;

            yield return null;
        }

        if (forward)
        {
            fromLayer.layer.gameObject.SetActive(false);
        }
        else
        {
            toLayer.layer.gameObject.SetActive(true);
        }
    }
}
