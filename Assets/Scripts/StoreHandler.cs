using System.Collections.Generic;
using Core;
using Player;
using Player.Stats.Meta;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoreHandler : MonoBehaviour
{
    [SerializeField] private Image upgradeImage;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private List<GameObject> upgradeButtons;
    [SerializeField] private TextMeshProUGUI onnaFragments;
    [SerializeField] private Image detailBackgroundImage;
    [SerializeField] private List<Sprite> levelBackgrounds;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip upgradeClip;
    private StoreUpgradeData selectedData;
    private BuyUpgradeButton selectedButton;
    private HubManager hub;
    
    private PlayerModel player;
    private PlayerInventory playerInventory;
    private PlayerModelBootstrapper playerModelBootstrapper;
    private bool showDebugPanel = false;

    private void Start() {
        
        StartCoroutine(DelayedCheck());  
        
        
        //Cursor Mouse
        Cursor.visible = true;
    }

    private IEnumerator DelayedCheck()
    {
        yield return null;
        playerInventory = PlayerHelper.GetPlayer().GetComponent<PlayerModel>().Inventory;
        CheckAvailableUpgrades();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerModelBootstrapperSignal>(GetModelBoostrapper);
    }
    
    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerModelBootstrapperSignal>(GetModelBoostrapper);
    }

    public void OnUpgradeClicked(BuyUpgradeButton button)
    {
        if (button.Data == null) return;

        int currentLevel = GetCurrentUpgradeLevel(button.Data);
        selectedData = button.Data;
        selectedButton = button;

        UpdateUpgradeDetail(button.Data, currentLevel);
        HandleBuyChance(button);
        AudioManager.Instance?.PlaySFX(buttonClickClip);
    }

    public void SetHubManager(HubManager hubManager) { hub = hubManager;}

    public void CloseStore()
    {
        playerInventory.PlayerItemsHolder.PrepareForSave();
        SaveSystem.SaveInventory(playerInventory);
        
        hub.CloseStore();

        //Cursor Mouse
        Cursor.visible = false;
    }

    private void UpdateCurrencyStatus()
    {
        onnaFragments.text = $"{playerInventory.PlayerWallet.Coins}";
    }

    private void CheckAvailableUpgrades()
    {
        UpdateCurrencyStatus();

        foreach (var buttonObj in upgradeButtons)
        {
            var upgradeButton = buttonObj.GetComponent<BuyUpgradeButton>();
            if (upgradeButton.Data == null) continue;

            int currentLevel = GetCurrentUpgradeLevel(upgradeButton.Data);
            upgradeButton.UpdateVisuals(currentLevel);

            var uiButton = buttonObj.GetComponent<Button>();
            //uiButton.interactable = true;
        }
    }

    private void HandleBuyChance(BuyUpgradeButton button)
    {
        int currentLevel = GetCurrentUpgradeLevel(button.Data);

        int index = Mathf.Clamp(currentLevel, 0, levelBackgrounds.Count - 1);
        detailBackgroundImage.sprite = levelBackgrounds[index];
        
        bool canUpgrade = playerInventory.PlayerItemsHolder.CanUpgrade(button.Data);
        int cost = button.Data.GetCost(currentLevel);

        upgradeButton.interactable = canUpgrade && CanAffordUpgrade(button.Data, currentLevel);
        upgradeCost.color = upgradeButton.interactable ? Color.white : Color.gray;
        upgradeCost.text = canUpgrade ? cost.ToString() : "MAX";
    }

    public void TryBuyUpgrade()
    {
        if (selectedData == null) return;

        int currentLevel = GetCurrentUpgradeLevel(selectedData);
        int cost = selectedData.GetCost(currentLevel);

        if (!playerInventory.PlayerItemsHolder.CanUpgrade(selectedData)) return;
        if (!playerInventory.PlayerWallet.TrySpend(cost)) return;

        player = PlayerHelper.GetPlayer().GetComponent<PlayerModel>();
        selectedData.UpgradeEffect?.Apply(player.StatContext.Meta, selectedData.GetValue(currentLevel), selectedData.Mode);
        playerInventory.PlayerItemsHolder.AddUpgrade(selectedData);
        AudioManager.Instance?.PlaySFX(upgradeClip);
        CheckAvailableUpgrades();
        HandleBuyChance(selectedButton);
    }

    private void GetModelBoostrapper(PlayerModelBootstrapperSignal signal)
    {
        playerModelBootstrapper = signal.Bootstrapper;
    }
    
    private int GetCurrentUpgradeLevel(StoreUpgradeData data)
    {
        playerInventory.PlayerItemsHolder.UpgradesBoughtDictionary.TryGetValue(data, out var level);
        return level;
    }

    private bool CanAffordUpgrade(StoreUpgradeData data, int level)
    {
        int cost = data.GetCost(level);
        return cost >= 0 && playerInventory.PlayerWallet.CheckCost(cost);
    }

    private void UpdateUpgradeDetail(StoreUpgradeData data, int level)
    {
        int cost = data.GetCost(level);
        upgradeCost.text = cost != -1 ? cost.ToString() : "MAX";
        upgradeImage.sprite = data.IconOnSelected;
        upgradeDescription.text = data.Description;
        upgradeName.text = data.UpgradeName;
    }

    private void OnGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 16 };

        float panelX = Screen.width - 170;
        float panelY = 10;
        float panelWidth = 160;
        float buttonHeight = 30;
        float spacing = 10;
        // Botón fijo en la esquina superior derecha
        if (GUI.Button(new Rect(panelX, panelY, panelWidth, buttonHeight), showDebugPanel ? "Ocultar Debug" : "Mostrar Debug", buttonStyle))
        {
            showDebugPanel = !showDebugPanel;
        }

        if (!showDebugPanel) return;

        // Panel de botones de debug

        if (GUI.Button(new Rect(panelX, panelY + 2 * (buttonHeight + spacing), panelWidth, buttonHeight), "➕ Add 100 Coins", buttonStyle))
        {
            playerInventory.PlayerWallet.AddCoins(100);
            Debug.Log("Added 100 coins!");
            UpdateCurrencyStatus();
        }

        if (GUI.Button(new Rect(panelX, panelY + 3 * (buttonHeight + spacing), panelWidth, buttonHeight), "🧹 Limpiar mejoras", buttonStyle))
        {
            playerInventory.PlayerItemsHolder.ClearUpgrades();
            playerInventory.PlayerItemsHolder.PrepareForSave(); 
            SaveSystem.SaveInventory(playerInventory);

            playerInventory = SaveSystem.Load().inventory;
            playerInventory.PlayerItemsHolder.RestoreFromSave();

            player.StatContext.Meta.Clear(); 
            playerInventory.PlayerItemsHolder.ApplyAllUpgradesTo(player.StatContext.Meta); 

            CheckAvailableUpgrades();
        }
    }
}