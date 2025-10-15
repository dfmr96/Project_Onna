using UnityEngine;

public class Engineer : InteractableBase
{
    [Header("Dialogue Data for the Engineer")]
    [SerializeField] private NPCData defaultDialogueData; // diálogo inicial
    [SerializeField] private NPCData nextDialogueData;    // diálogo que se activa después

    [Header("Optional: Reference to Engineer object (for animations, etc.)")]
    [SerializeField] private GameObject engineerObject;

    // Mantiene el diálogo actual que se usará
    private NPCData currentDialogueData;

    private void Awake()
    {
        currentDialogueData = defaultDialogueData;
    }

    public override void Interact()
    {
        base.Interact();

        if (currentDialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(currentDialogueData, this);
        }
        else
        {
            Debug.LogWarning($"{name}: No NPCData assigned for Engineer dialogue.");
        }

        if (engineerObject != null)
        {
            Animator anim = engineerObject.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("StartDialogue"); // opcional, depende de tu Animator
        }
    }

    // Método para cambiar el diálogo al siguiente
    public void SetDialogueToNext()
    {
        if (nextDialogueData != null)
        {
            currentDialogueData = nextDialogueData;
            Debug.Log("Engineer dialogue changed to next data.");
        }
    }

    // Devuelve el diálogo actual (para DialogueUI)
    public NPCData GetNextDialogue()
    {
        return nextDialogueData;
    }
}
