using System.Collections.Generic;
using Core;
using Player;
using Player.Stats.Meta;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreHandler : MonoBehaviour
{
    [SerializeField] private Image upgradeImage;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private List<GameObject> upgradeButtons;
    [SerializeField] private TextMeshProUGUI onnaFragments;
    private UpgradeData selectedData;
    private HubManager hub;
    
    private PlayerModel player;
    private PlayerModelBootstrapper playerModelBootstrapper;

    private void Start() { CheckUpgradesAvailables(); }

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
        upgradeImage.sprite = button.Data.Icon;
        upgradeDescription.text = button.Data.Description;
        upgradeName.text = button.Data.UpgradeName;
        upgradeCost.text = button.Data.Cost.ToString();
        selectedData = button.Data;
        HandleBuyChance(button);
    }

    public void SetHubManager(HubManager hubManager) { hub = hubManager; }

    public void CloseStore()
    {
        MetaStatSaveSystem.Save(playerModelBootstrapper.MetaStats, playerModelBootstrapper.Registry);
        hub.CloseStore();
    }
    public void UpdateCurrencyStatus() { onnaFragments.text = "Onna Fragments: " + hub.PlayerInventory.PlayerWallet.Coins; }
    public void CheckUpgradesAvailables()
    {
        UpdateCurrencyStatus();
        foreach (GameObject button in upgradeButtons)
        {
            button.GetComponent<Button>().interactable = false;
            if (button.GetComponent<BuyUpgradeButton>().Data != null)
            {
                button.GetComponent<Button>().interactable = true;
                HandleBuyChance(button.GetComponent<BuyUpgradeButton>());
            }
        }
    }

    private void HandleBuyChance(BuyUpgradeButton button)
    {
        
        if (!hub.PlayerInventory.PlayerWallet.CheckCost(button.Data.Cost))
        { 
            upgradeButton.interactable = false;
            upgradeCost.color = Color.gray;
        }
        else 
        { 
            upgradeButton.interactable = true;
            upgradeCost.color = Color.white;
        }
    }


    public void TryBuyUpgrade()
    {
        if (selectedData != null)
        {
            if (hub.PlayerInventory.PlayerWallet.TrySpend(selectedData.Cost))
            {
                player = PlayerHelper.GetPlayer().GetComponent<PlayerModel>();
                Debug.Log($"Compraste mejora: {selectedData.UpgradeName}");
                selectedData.UpgradeEffect?.Apply(player.StatContext.Meta);
                hub.PlayerInventory.PlayerItemsHolder.AddUpgrade(selectedData);
                hub.UpdateCoins();
                CheckUpgradesAvailables();
            }
        }
    }
    
    private void GetModelBoostrapper(PlayerModelBootstrapperSignal signal)
    {
        playerModelBootstrapper = signal.Bootstrapper;
    }
}
