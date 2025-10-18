using UnityEngine;
using Player;

public class PlayerTutorialTracker : MonoBehaviour
{
    [Header("Referencia al Input")]
    [SerializeField] private PlayerInputHandler inputHandler;

    [Header("Referencia al WeaponDialogueTrigger para el siguiente diálogo")]
    [SerializeField] private WeaponDialogueTrigger weaponTrigger;

    [Header("NPCData a abrir después del checklist")]
    [SerializeField] private NPCData nextDialogueData;

    [Header("Referencia al EnemySpawner")]
    [SerializeField] private GameObject enemySpawnerGO;

    private bool isCountingInputs = false;

    private bool hasFired;
    private bool hasMelee;
    private bool hasReloaded;
    private bool hasDashed;
    private bool hasMoved;

    private bool checklistCompleted => hasFired && hasMelee && hasReloaded && hasDashed && hasMoved;

    private void OnDisable()
    {
        if (inputHandler != null)
        {
            inputHandler.FirePerformed -= OnFire;
            inputHandler.MeleeAtackPerformed -= OnMelee;
            inputHandler.ReloadPerformed -= OnReload;
            inputHandler.DashPerformed -= OnDash;
        }
    }

    private void OnDestroy()
    {
        if (enemySpawnerGO != null)
        {
            EnemySpawner spawner = enemySpawnerGO.GetComponent<EnemySpawner>();
            if (spawner != null)
                spawner.OnAllWavesCompleted -= OnAllWavesCompleted;
        }
    }


    private void Start()
    {
        if (inputHandler == null)
            inputHandler = PlayerInputHandler.Instance;

        if (inputHandler != null)
        {
            inputHandler.FirePerformed += OnFire;
            inputHandler.MeleeAtackPerformed += OnMelee;
            inputHandler.ReloadPerformed += OnReload;
            inputHandler.DashPerformed += OnDash;
        }

        // 🔹 Si el spawner ya está referenciado, suscribimos al evento
        if (enemySpawnerGO != null)
        {
            EnemySpawner spawner = enemySpawnerGO.GetComponent<EnemySpawner>();
            if (spawner != null)
                spawner.OnAllWavesCompleted += OnAllWavesCompleted;
        }
    }


    private void Update()
    {
        if (!isCountingInputs || inputHandler == null) return;

        if (!hasMoved && inputHandler.RawMovementInput.magnitude > 0.1f)
            OnMove();

        if (checklistCompleted)
            CompleteChecklist();
    }

    private void OnFire() { if (isCountingInputs) hasFired = true; CompleteIfDone(); }
    private void OnMelee() { if (isCountingInputs) hasMelee = true; CompleteIfDone(); }
    private void OnReload() { if (isCountingInputs) hasReloaded = true; CompleteIfDone(); }
    private void OnDash() { if (isCountingInputs) hasDashed = true; CompleteIfDone(); }
    private void OnMove() { if (isCountingInputs) hasMoved = true; CompleteIfDone(); }

    private void CompleteIfDone()
    {
        if (checklistCompleted)
            CompleteChecklist();
    }
    
    private void OnAllWavesCompleted()
    {
        Debug.Log("✅ Todas las oleadas completadas, abriendo diálogo final.");
        if (weaponTrigger != null)
            weaponTrigger.StartDefeatedEnemiesDialogue();
    }


    private void CompleteChecklist()
    {
        enabled = false;

        if (weaponTrigger != null && nextDialogueData != null)
        {
            weaponTrigger.SetDialogueData(nextDialogueData);

            // 👇 aseguramos que el DialogueManager use la data actualizada
            DialogueManager.Instance.SetCurrentNPCData(nextDialogueData);

            // Abrimos el diálogo final del arma
            DialogueManager.Instance.StartDialogue(nextDialogueData, weaponTrigger);

            // 🔹 Esperamos a que cierre el diálogo antes de empezar a contar
            DialogueManager.Instance.CurrentTriggerActionOnEnd = () =>
            {
                StartCountingInputs();
            };
        }
    }

    public void StartCountingInputs()
    {
        isCountingInputs = true;
        enabled = true; // Update empieza a correr
    }
    // Función pública que se llama desde el Option del diálogo
    public void ActivateEnemySpawner()
    {
        if (enemySpawnerGO != null)
            enemySpawnerGO.SetActive(true);
        else
            Debug.LogWarning("ActivateEnemySpawner: EnemySpawner GO no asignado");
    }
}
