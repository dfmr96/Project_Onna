using UnityEngine;

public class Soldier_Dialogue_Trigger : InteractableBase
{
    [Header("Dialogue Data")]
    [SerializeField] public NPCData dialogueData;

    public override void Interact()
    {
        base.Interact();

        if (dialogueData == null)
        {
            Debug.LogWarning("Soldier_Dialogue_Trigger: No NPCData assigned.");
            return;
        }

        // Iniciamos el diálogo
        DialogueManager.Instance.StartDialogue(dialogueData, this);
    }
}
