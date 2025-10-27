using Player;
using Player.Stats.Runtime;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public static HubManager Instance;

    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] private GameObject storePrefab;
    [SerializeField] private GameObject pausePrefab;
    [SerializeField] private AudioClip gameMusicClip;
    private GameObject storeInstance = null;
    private GameObject pauseInstance = null;

    private PlayerInventory playerInventory;
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

    private void Start() => AudioManager.Instance?.PlayMusic(gameMusicClip);

    public void Init()
    {
        levelProgression.ResetProgress();
        playerInventory = PlayerHelper.GetPlayer()?.GetComponent<PlayerModel>().Inventory;
        PlayerHelper.GetPlayer().GetComponent<PlayerController>().HandlePauseAccess += TogglePauseMenu;

        // Si venimos de una run con monedas, las sumamos
        if (RunData.CurrentCurrency != null)
        {
            playerInventory.PlayerWallet.AddCoins(RunData.CurrentCurrency.Coins);
            SaveSystem.SaveInventory(playerInventory);
            RunData.Clear();
        }
    }

    //private void OnDisable() => PlayerHelper.GetPlayer().GetComponent<PlayerController>().HandlePauseAccess -= TogglePauseMenu;

    public void OpenStore()
    {
        if (storeInstance != null) return;
         PlayerHelper.DisableInput();
        storeInstance = Instantiate(storePrefab);
        StoreHandler handler = storeInstance.GetComponent<StoreHandler>();
        handler.SetHubManager(this);
       
        CursorHelper.Show();
    }

    public void CloseStore()
    {
        if (storeInstance != null)
        {
            Destroy(storeInstance);
            storeInstance = null;

            PlayerHelper.EnableInput();

            if (DialogueManager.Instance == null || DialogueManager.Instance.CurrentTrigger == null)
                Cursor.visible = false;
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseInstance == null)
        {
            pauseInstance = Instantiate(pausePrefab);
            pauseInstance.SetActive(false);
        }
        pauseInstance.SetActive(!pauseInstance.activeInHierarchy);
    }

    [ContextMenu("Add Currency")]
    private void ApplyCurrency()
    {
        playerInventory.PlayerWallet.AddCoins(100);
    }
}
