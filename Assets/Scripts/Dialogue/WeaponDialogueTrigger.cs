using UnityEngine;

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

    private bool hasTriggered = false; // control de primera vez

    protected override void Start() { /* no queremos el prefab */ }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; // ya se activó antes
        if (!other.CompareTag("Player")) return;

        hasTriggered = true; // marcamos que ya pasó por el trigger
        
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        DialogueManager.Instance.StartDialogue(weaponDialogueData, this);
    }

    public void HandleAction(string action)
    {
        if (action == actionId)
        {
            // Inicia la secuencia con el nuevo data después de 5 segundos
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

    public void SetDialogueData(NPCData newData)
    {
        weaponDialogueData = newData;
    }
}
