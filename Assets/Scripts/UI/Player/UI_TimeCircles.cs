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

    private int lastMilestone = 0;

    private void Awake()
    {
        // Inicializamos los estados de los indicadores
        for (int i = 0; i < lifeIndicators.Count; i++)
        {
            indicatorStates.Add(false);
        }
    }

    public void UpdateTimeUI(float timePercent)
    {
        var player = PlayerHelper.GetPlayer()?.GetComponent<PlayerModel>();
        if (player == null) return;

        float currentTime = player.CurrentHealth;
        float maxTime = player.MaxHealth;

        UpdateCircles(currentTime, maxTime);
    }

    private void UpdateCircles(float currentTime, float maxTime)
    {
        int totalCircles = Mathf.Min(timeCircles.Count, Mathf.CeilToInt(maxTime / secondsPerCircle));

        for (int i = 0; i < timeCircles.Count; i++)
        {
            Image circle = timeCircles[i];
            GameObject indicator = (lifeIndicators != null && i < lifeIndicators.Count) ? lifeIndicators[i] : null;

            if (i < totalCircles)
            {
                circle.gameObject.SetActive(true);

                float segmentStart = i * secondsPerCircle;

                // Calcula fill entre minFill y maxFill
                float fill = Mathf.Clamp01((currentTime - segmentStart) / secondsPerCircle);
                circle.fillAmount = Mathf.Lerp(minFill, maxFill, fill);

                if (indicator != null)
                {
                    bool shouldBeOn = fill > 0f;

                    // Ejecutamos animación solo si cambió el estado
                    if (indicatorStates[i] != shouldBeOn)
                    {
                        var anim = indicator.GetComponent<Animator>();
                        if (anim != null)
                        {
                            anim.SetTrigger(shouldBeOn ? "TurnOn" : "TurnOff");
                        }
                        indicatorStates[i] = shouldBeOn;
                    }
                }
            }
            else
            {
                circle.gameObject.SetActive(false);

                if (indicator != null && indicatorStates[i])
                {
                    // Si estaba prendida y ahora se apaga
                    var anim = indicator.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.SetTrigger("TurnOff");
                    }
                    indicatorStates[i] = false;
                }
            }
        }
    }
}
