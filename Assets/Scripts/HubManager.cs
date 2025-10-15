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

    // 🔹 Propiedad útil para que DialogueManager sepa si la tienda está abierta
    public bool IsStoreOpen => storeInstance != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        AudioManager.Instance?.PlayMusic(gameMusicClip);
    }

    public void Init()
    {
        levelProgression.ResetProgress();
        playerInventory = PlayerHelper.GetPlayer().GetComponent<PlayerModel>().Inventory;

        // Si venimos de una run con monedas, las sumamos
        if (RunData.CurrentCurrency != null)
        {
            playerInventory.PlayerWallet.AddCoins(RunData.CurrentCurrency.Coins);
            SaveSystem.SaveInventory(playerInventory);
            RunData.Clear();
        }

        UpdateCoins();
    }

    public void UpdateCoins()
    {
        currencyText.text = $"Onna Fragments: {playerInventory.PlayerWallet.Coins}";
    }

    public void OpenStore()
    {
        if (storeInstance != null) return;
         PlayerHelper.DisableInput();
        storeInstance = Instantiate(storePrefab);
        StoreHandler handler = storeInstance.GetComponent<StoreHandler>();
        handler.SetHubManager(this);

        // Bloquea movimiento del jugador y muestra cursor
       
        CursorHelper.Show();
    }

    public void CloseStore()
    {
        if (storeInstance != null)
        {
            Destroy(storeInstance);
            storeInstance = null;

            // Reactiva movimiento del jugador
            PlayerHelper.EnableInput();

            // Oculta el cursor si no hay otra UI abierta (como un diálogo)
            if (DialogueManager.Instance == null || DialogueManager.Instance.CurrentTrigger == null)
                Cursor.visible = false;
        }
    }

    [ContextMenu("Add Currency")]
    private void ApplyCurrency()
    {
        playerInventory.PlayerWallet.AddCoins(100);
        UpdateCoins();
    }
}
