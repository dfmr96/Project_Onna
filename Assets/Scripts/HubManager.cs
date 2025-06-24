using Player;
using Player.Stats.Runtime;
using UnityEngine;
using TMPro;

public class HubManager : MonoBehaviour
{
    public static HubManager Instance;

    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private GameObject storePrefab;
    private GameObject storeInstance;
    private PlayerInventory _playerInventory;
    public PlayerInventory PlayerInventory => _playerInventory;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void Init()
    {
        levelProgression.ResetProgress();
        _playerInventory = new PlayerInventory();

        // Si venimos de una run con monedas las sumamos
        if (RunData.CurrentCurrency != null)
        {
            PlayerInventory.PlayerWallet.AddCoins(RunData.CurrentCurrency.Coins);
            RunData.Clear();
        }
        UpdateCoins();
    }
    public void UpdateCoins() { currencyText.text = "Onna Fragments: " + PlayerInventory.PlayerWallet.Coins.ToString(); }
    public void OpenStore()
    {
        if (storeInstance != null) return;

        storeInstance = Instantiate(storePrefab);
        StoreHandler handler = storeInstance.GetComponent<StoreHandler>();
        handler.SetHubManager(this);
        PlayerHelper.DisableInput();
    }

    public void CloseStore()
    {
        if (storeInstance != null)
        {
            Destroy(storeInstance);
            storeInstance = null;
            PlayerHelper.EnableInput();
        }
    }

    [ContextMenu("Add Currency")]
    void ApplyCurrency() { PlayerInventory.PlayerWallet.AddCoins(100); UpdateCoins(); }
}
