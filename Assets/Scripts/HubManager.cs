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
    private PlayerWallet _playerWallet;
    public PlayerWallet PlayerWallet => _playerWallet;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    public void Init()
    {
        levelProgression.ResetProgress();
        InitWallet();
        
        // Si venimos de una run con monedas las sumamos
        if (RunData.CurrentCurrency != null)
        {
            PlayerWallet.AddCoins(RunData.CurrentCurrency.Coins);
            RunData.Clear();
        }
        UpdateCoins();
    }
    
    private void InitWallet()
    {
        if (_playerWallet == null)
            _playerWallet = new PlayerWallet();
    }
    public void UpdateCoins() { currencyText.text = "Onna Fragments: " + PlayerWallet.Coins.ToString(); }
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
    void ApplyCurrency() { PlayerWallet.AddCoins(100); UpdateCoins(); }
}
