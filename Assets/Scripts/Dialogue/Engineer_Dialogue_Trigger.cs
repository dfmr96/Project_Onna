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
    private bool hasEndedDialogue = false;

    private void Awake() => currentDialogueData = defaultDialogueData;

    public override void Interact()
    {
        base.Interact();

        if (hasEndedDialogue && loopDialogueData != null)
            currentDialogueData = loopDialogueData;


        if (currentDialogueData != null)
            DialogueManager.Instance.StartDialogue(currentDialogueData, this);

        if (engineerObject != null) { Animator anim = engineerObject.GetComponent<Animator>(); }
    }

    public void SetDialogueToNext()
    {
        if (nextDialogueData != null)
            currentDialogueData = nextDialogueData;
    }

    public void SetDialogueToLoop()
    {
        if (loopDialogueData != null)
        {
            currentDialogueData = loopDialogueData;
            hasEndedDialogue = true;
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
    }

    public NPCData GetNextDialogue() => nextDialogueData;
}
