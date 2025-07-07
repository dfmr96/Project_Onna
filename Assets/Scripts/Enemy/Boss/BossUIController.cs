using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUIController : MonoBehaviour
{
    [Header("Boss Health")]
    [SerializeField] private Image bossHealthSlider;

    [Header("Pillar Health")]
    [SerializeField] private List<Image> pillarHealthFills; 

    public void UpdateBossHealth(float current, float max)
    {
        bossHealthSlider.fillAmount = current / max;

        if(bossHealthSlider.fillAmount <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdatePillarHealth(int pillarIndex, float current, float max)
    {
        if (pillarIndex < 0 || pillarIndex >= pillarHealthFills.Count) return;

        pillarHealthFills[pillarIndex].fillAmount = current / max;
    }
}

