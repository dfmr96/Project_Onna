using UnityEngine;

public class Engineer_Dialogue_Trigger : InteractableBase
{
    [Header("Dialogue Data for the Engineer")]
    [SerializeField] private NPCData defaultDialogueData;
    [SerializeField] private NPCData nextDialogueData;
    [SerializeField] private NPCData loopDialogueData;

    [Header("Optional: Reference to Engineer object (for animations, etc.)")]
    [SerializeField] private GameObject engineerObject;

    [Header("Objects to Activate/Deactivate at the end")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    private NPCData currentDialogueData;

    private void Awake()
    {
        currentDialogueData = defaultDialogueData;
    }

    public override void Interact()
    {
        base.Interact();

        if (currentDialogueData != null)
            DialogueManager.Instance.StartDialogue(currentDialogueData, this);
        else
            Debug.LogWarning($"{name}: No NPCData assigned for Engineer dialogue.");

        if (engineerObject != null)
        {
            Animator anim = engineerObject.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("StartDialogue");
        }
    }

    public void SetDialogueToNext()
    {
        if (nextDialogueData != null)
        {
            currentDialogueData = nextDialogueData;
            Debug.Log("Engineer dialogue changed to next data.");
        }
    }

    public void SetDialogueToLoop()
    {
        if (loopDialogueData != null)
        {
            currentDialogueData = loopDialogueData;
            Debug.Log("Engineer dialogue changed to loop data.");
        }
    }

    public void OnEngineerEndAction()
    {
        SetDialogueToLoop();

        foreach (var obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }
        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null) obj.SetActive(false);
        }

        Debug.Log("Engineer end action executed: Dialogue set to loop and scene objects updated.");
    }

    public NPCData GetNextDialogue() => nextDialogueData;

}
