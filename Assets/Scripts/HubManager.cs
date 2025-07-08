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
    [SerializeField] private AudioClip gameMusicClip;
    private GameObject storeInstance;
    
    private PlayerInventory playerInventory;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void Start() { AudioManager.Instance?.PlayMusic(gameMusicClip); }

    public void Init()
    {
        levelProgression.ResetProgress();
        playerInventory = PlayerHelper.GetPlayer().GetComponent<PlayerModel>().Inventory;
        // Si venimos de una run con monedas las sumamos
        if (RunData.CurrentCurrency != null)
        {
            playerInventory.PlayerWallet.AddCoins(RunData.CurrentCurrency.Coins);
            RunData.Clear();
        } //TODO Esto no debe ir aqui
        UpdateCoins();
    }

    public void UpdateCoins()
    {
        currencyText.text = "Onna Fragments: " + playerInventory.PlayerWallet.Coins.ToString();
    }
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
    void ApplyCurrency()
    {
        playerInventory.PlayerWallet.AddCoins(100); UpdateCoins();
    }
}
