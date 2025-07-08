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
    private UpgradeData selectedData;
    private BuyUpgradeButton selectedButton;
    private HubManager hub;
    
    private PlayerModel player;
    private PlayerModelBootstrapper playerModelBootstrapper;

    private void Start() { StartCoroutine(DelayedCheck()); }

    private IEnumerator DelayedCheck()
    {
        yield return null;
        SaveSystem.DebugInventoryJson();
        hub.PlayerInventory = SaveSystem.LoadInventory();
        CheckUpgradesAvailables();
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
        upgradeImage.sprite = button.Data.Icon;
        upgradeDescription.text = button.Data.Description;
        upgradeName.text = button.Data.UpgradeName;
        upgradeCost.text = button.Data.Cost.ToString();
        selectedData = button.Data;
        selectedButton = button;
        AudioManager.Instance?.PlayOneShot(buttonClickClip);
        HandleBuyChance(selectedButton);
    }

    public void SetHubManager(HubManager hubManager) { hub = hubManager;}

    public void CloseStore()
    {
        MetaStatSaveSystem.Save(playerModelBootstrapper.MetaStats, playerModelBootstrapper.Registry);
        
        hub.PlayerInventory.PlayerItemsHolder.PrepareForSave();
        SaveSystem.SaveInventory(HubManager.Instance.PlayerInventory);
        
        hub.CloseStore();
    }
    public void UpdateCurrencyStatus() { onnaFragments.text = "Onna Fragments: " + hub.PlayerInventory.PlayerWallet.Coins; }
    public void CheckUpgradesAvailables()
    {
        UpdateCurrencyStatus();

        foreach (GameObject button in upgradeButtons)
        {
            var data = button.GetComponent<BuyUpgradeButton>().Data;

            if (data == null) continue;

            button.GetComponent<Button>().interactable = data != null;

            int currentLevel = 0;
            hub.PlayerInventory.PlayerItemsHolder.UpgradesBoughtDictionary.TryGetValue(data, out currentLevel);

            button.GetComponent<BuyUpgradeButton>().UpdateVisuals(currentLevel);
        }
    }

    private void HandleBuyChance(BuyUpgradeButton button)
    {
        int currentLevel = 0;
        hub.PlayerInventory.PlayerItemsHolder.UpgradesBoughtDictionary.TryGetValue(button.Data, out currentLevel);

        int index = Mathf.Clamp(currentLevel, 0, levelBackgrounds.Count - 1);
        detailBackgroundImage.sprite = levelBackgrounds[index];

        upgradeButton.interactable = hub.PlayerInventory.PlayerItemsHolder.CanUpgrade(button.Data) && 
            hub.PlayerInventory.PlayerWallet.CheckCost(button.Data.Cost);
        upgradeCost.color = upgradeButton.interactable ? Color.white : Color.gray;
        upgradeCost.text = hub.PlayerInventory.PlayerItemsHolder.CanUpgrade(button.Data) ? button.Data.Cost.ToString() : "MAX";
    }

    public void TryBuyUpgrade()
    {
        if (selectedData == null) return;

        if (!hub.PlayerInventory.PlayerItemsHolder.CanUpgrade(selectedData)) return;
        if (!hub.PlayerInventory.PlayerWallet.TrySpend(selectedData.Cost)) return;

        player = PlayerHelper.GetPlayer().GetComponent<PlayerModel>();
        selectedData.UpgradeEffect?.Apply(player.StatContext.Meta);
        hub.PlayerInventory.PlayerItemsHolder.AddUpgrade(selectedData);
        hub.UpdateCoins();
        AudioManager.Instance?.PlayOneShot(upgradeClip);
        CheckUpgradesAvailables();
        HandleBuyChance(selectedButton);
    }

    private void GetModelBoostrapper(PlayerModelBootstrapperSignal signal)
    {
        playerModelBootstrapper = signal.Bootstrapper;
    }
    
    private void OnGUI()
    {

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;

        if (GUI.Button(new Rect(20, 20, 160, 40), "Add 100 Coins", buttonStyle))
        {
            PlayerInventory inventory = hub.PlayerInventory;
            inventory.PlayerWallet.AddCoins(100);

            Debug.Log("Added 100 coins!");
            UpdateCurrencyStatus();
        }
    }
}