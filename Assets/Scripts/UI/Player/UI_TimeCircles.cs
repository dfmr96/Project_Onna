using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;

public class UI_TimeCircles : MonoBehaviour
{
    [Header("Time Circles (vida por segmentos)")]
    [SerializeField] private List<Image> timeCircles;
    [SerializeField] private int secondsPerCircle = 60;
    [SerializeField] private List<GameObject> lifeIndicators; // indicadores de luz
    private List<bool> indicatorStates = new List<bool>(); // guardamos estado previo

    float minFill = 0.06f;
    float maxFill = 0.94f;

    [Header("Milestone Animations & Shake")]
    [SerializeField] private Animator milestoneAnimator;
    [SerializeField] private UI_Shake uiShake;

    [Header("Blink Effect")]
    [SerializeField] private Color blinkColor = Color.white; // color del titileo
    [SerializeField] private float blinkSpeed = 3f; // velocidad del titileo
    private List<Color> originalColors = new List<Color>(); // para guardar el color original de cada círculo

    private void Awake()
    {
        // Inicializamos los estados de los indicadores y colores originales
        for (int i = 0; i < lifeIndicators.Count; i++)
        {
            indicatorStates.Add(false);
        }
        foreach (var circle in timeCircles)
        {
            originalColors.Add(circle.color);
        }
    }

    public void UpdateTimeUI(float timePercent)
    {
        // Evitar ejecución si el objeto o la UI no están activos
        if (!gameObject.activeInHierarchy) return;

        var player = PlayerHelper.GetPlayer()?.GetComponent<PlayerModel>();
        if (player == null) return;

        float currentTime = player.CurrentHealth;
        float maxTime = player.MaxHealth;

        // Evitar procesamiento si los círculos están ocultos o vacíos
        if (timeCircles == null || timeCircles.Count == 0)
            return;

        UpdateCircles(currentTime, maxTime);
        UpdateBlink(currentTime);
    }

    private void UpdateCircles(float currentTime, float maxTime)
    {
        if (!gameObject.activeInHierarchy) return;

        int totalCircles = Mathf.Min(timeCircles.Count, Mathf.CeilToInt(maxTime / secondsPerCircle));

        for (int i = 0; i < timeCircles.Count; i++)
        {
            Image circle = timeCircles[i];
            if (circle == null) continue; // si el objeto fue destruido o desactivado

            GameObject indicator = (lifeIndicators != null && i < lifeIndicators.Count) ? lifeIndicators[i] : null;

            if (i < totalCircles)
            {
                circle.gameObject.SetActive(true);

                float segmentStart = i * secondsPerCircle;
                float fill = Mathf.Clamp01((currentTime - segmentStart) / secondsPerCircle);

                circle.fillAmount = Mathf.Lerp(minFill, maxFill, fill);

                if (indicator != null)
                {
                    bool shouldBeOn = fill > 0f;

                    if (indicatorStates[i] != shouldBeOn)
                    {
                        var anim = indicator.GetComponent<Animator>();
                        if (anim != null)
                            anim.SetTrigger(shouldBeOn ? "TurnOn" : "TurnOff");

                        indicatorStates[i] = shouldBeOn;
                    }
                }
            }
            else
            {
                if (circle.gameObject.activeSelf)
                    circle.gameObject.SetActive(false);

                if (indicator != null && indicatorStates[i])
                {
                    var anim = indicator.GetComponent<Animator>();
                    if (anim != null)
                        anim.SetTrigger("TurnOff");

                    indicatorStates[i] = false;
                }
            }
        }
    }

    private void UpdateBlink(float currentTime)
    {
        if (!gameObject.activeInHierarchy) return;

        int circleIndex = Mathf.FloorToInt(currentTime / secondsPerCircle);
        if (circleIndex < 0 || circleIndex >= timeCircles.Count) return;

        Image circle = timeCircles[circleIndex];
        if (circle == null || !circle.gameObject.activeInHierarchy) return;

        float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        circle.color = Color.Lerp(originalColors[circleIndex], blinkColor, t);
    }

    public void ShakeOnDamage()
    {
        uiShake?.Shake(0.25f, 8f); // Shake más intenso
    }
}
