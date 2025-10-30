using UnityEngine;
using Player;
using System.Collections;

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
    [SerializeField] private GameObject playerHealthUI;

    [Header("Tiempo límite para repetir diálogo (segundos)")]
    [SerializeField] private float reminderTime = 10f;

    private bool isCountingInputs = false;

    private bool hasFired;
    private bool hasMelee;
    private bool hasReloaded;

    private bool checklistCompleted => hasFired && hasMelee && hasReloaded;
    private Coroutine reminderCoroutine;

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

        if (enemySpawnerGO != null)
        {
            EnemySpawner spawner = enemySpawnerGO.GetComponent<EnemySpawner>();
            if (spawner != null)
                spawner.OnAllWavesCompleted += OnAllWavesCompleted;
        }
    }

    private void Update()
    {
        if (!isCountingInputs) return;
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
        if (weaponTrigger != null)
            weaponTrigger.StartDefeatedEnemiesDialogue();
    }

    private void SetPlayerInvulnerable(bool state)
    {
        var player = PlayerHelper.GetPlayer();
        if (player == null) return;

        var model = player.GetComponent<PlayerModel>();
        if (model != null)
            model.SetInvulnerable(state);
    }


    private void CompleteChecklist()
    {
        isCountingInputs = false;
        enabled = false;


        if (reminderCoroutine != null)
        {
            StopCoroutine(reminderCoroutine);
            reminderCoroutine = null;
        }

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
        enabled = true;

        hasFired = hasMelee = hasReloaded = false;

        if (reminderCoroutine != null)
            StopCoroutine(reminderCoroutine);

        reminderCoroutine = StartCoroutine(ReminderTimer());
    }

    private IEnumerator ReminderTimer()
    {
        yield return new WaitForSeconds(reminderTime);

        if (!checklistCompleted && weaponTrigger != null)
        {
            var currentData = weaponTrigger.GetCurrentDialogueData();

            if (currentData != null)
            {
                DialogueManager.Instance.SetCurrentNPCData(currentData);
                DialogueManager.Instance.StartDialogue(currentData, weaponTrigger);
                DialogueManager.Instance.CurrentTriggerActionOnEnd = () =>
                {
                    StartCountingInputs();
                };
            }
        }

    }

    public void ActivateEnemySpawner()
    {
        if (enemySpawnerGO != null)
            enemySpawnerGO.SetActive(true);
        else
            Debug.LogWarning("ActivateEnemySpawner: EnemySpawner GO no asignado");

        // Parpadeo de la UI de vida
        SetPlayerInvulnerable(false);
        if (playerHealthUI != null)
            StartCoroutine(BlinkPlayerHealthUI());
    }

    private IEnumerator BlinkPlayerHealthUI()
    {
        CanvasGroup cg = playerHealthUI.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = playerHealthUI.AddComponent<CanvasGroup>();

        // 3 parpadeos rápidos
        for (int i = 0; i < 3; i++)
        {
            cg.alpha = 0f;
            yield return new WaitForSeconds(0.2f);
            cg.alpha = 1f;
            yield return new WaitForSeconds(0.2f);
        }

        cg.alpha = 1f; // Queda visible al final
    }

}
