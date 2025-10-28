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

    private bool checklistCompleted => hasFired && hasMelee && hasReloaded;

    private void OnDisable()
    {
        if (inputHandler != null)
        {
            inputHandler.FirePerformed -= OnFire;
            inputHandler.MeleeAtackPerformed -= OnMelee;
            inputHandler.ReloadPerformed -= OnReload;
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


        if (checklistCompleted)
            CompleteChecklist();
    }

    private void OnFire() { if (isCountingInputs) hasFired = true; CompleteIfDone(); }
    private void OnMelee() { if (isCountingInputs) hasMelee = true; CompleteIfDone(); }
    private void OnReload() { if (isCountingInputs) hasReloaded = true; CompleteIfDone(); }

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

            DialogueManager.Instance.SetCurrentNPCData(nextDialogueData);
            DialogueManager.Instance.StartDialogue(nextDialogueData, weaponTrigger);
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
