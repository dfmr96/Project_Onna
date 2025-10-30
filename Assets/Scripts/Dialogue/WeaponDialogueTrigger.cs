using UnityEngine;
using Player;

public class WeaponDialogueTrigger : InteractableBase
{
    [Header("Datos del diálogo del arma")]
    [SerializeField] private NPCData weaponDialogueData;

    [Header("Próximo diálogo tras el ActionID")]
    [SerializeField] private NPCData nextDialogueData;

    [Header("Próximo diálogo tras enemysDeafet")]
    [SerializeField] public NPCData defeatedEnemiesDialogue;

    [Header("Action ID que dispara evento especial")]
    [SerializeField] private string actionId = "ONNAPreTutorial";

    public NPCData GetCurrentDialogueData() => weaponDialogueData;


    private bool hasTriggered = false; 

    protected override void Start()
    {
        var player = PlayerHelper.GetPlayer();
        if (player != null)
        {
            var model = player.GetComponent<PlayerModel>();
            if (model != null)
                model.SetInvulnerable(true);
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (hasTriggered) return; 
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        DialogueManager.Instance.StartDialogue(weaponDialogueData, this);
    }


    public void HandleAction(string action)
    {
        if (action == actionId)
        {
            DialogueManager.Instance.StartCoroutine(
                DialogueManager.Instance.PreTutorialTimer(nextDialogueData, this)
            );
        }
    }

    public void StartDefeatedEnemiesDialogue()
    {
        if (defeatedEnemiesDialogue != null)
        {
            DialogueManager.Instance.SetCurrentNPCData(defeatedEnemiesDialogue);
            DialogueManager.Instance.StartDialogue(defeatedEnemiesDialogue, this);
        }
        else
        {
            Debug.LogWarning("WeaponDialogueTrigger: defeatedEnemiesDialogue no asignado.");
        }
    }


    public NPCData GetNextDialogue() => nextDialogueData;

    public void SetDialogueData(NPCData newData) => weaponDialogueData = newData;
}
