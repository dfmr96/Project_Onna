using ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyUpgradeButton : MonoBehaviour
{
    [SerializeField] private StoreUpgradeData data;
    [SerializeField] private Image img;
    [SerializeField] private Sprite lockSprite;
    private Image backgroundImage;
    [SerializeField] private List<Sprite> levelBackgrounds;
    public StoreUpgradeData Data => data;
    private void Start() 
    {
        if (data != null) img.sprite = data.Icon;
        else img.sprite = lockSprite;
        backgroundImage = GetComponent<Image>();
    }

    public void UpdateVisuals(int currentLevel)
    {
        if (data == null) return;

        if (backgroundImage == null) return;

        if (levelBackgrounds == null || levelBackgrounds.Count == 0) return;

        int index = Mathf.Clamp(currentLevel, 0, levelBackgrounds.Count - 1);
        backgroundImage.sprite = levelBackgrounds[index];
    }
}
