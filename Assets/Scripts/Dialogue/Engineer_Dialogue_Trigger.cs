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
    private bool hasEndedDialogue = false; // controla si ya se cambió al loop

    private void Awake()
    {
        currentDialogueData = defaultDialogueData;
    }

    public override void Interact()
    {
        base.Interact();

        // Asegura que se mantenga el loop si ya terminó
        if (hasEndedDialogue && loopDialogueData != null)
        {
            currentDialogueData = loopDialogueData;
            //Debug.Log("[Engineer] Already ended dialogue, using loop dialogue.");
        }

        //Debug.Log($"[Engineer] Interact called. Current dialogue: {currentDialogueData?.name}");

        if (currentDialogueData != null)
            DialogueManager.Instance.StartDialogue(currentDialogueData, this);
        else
            //Debug.LogWarning($"{name}: No NPCData assigned for Engineer dialogue.");

        if (engineerObject != null)
        {
            Animator anim = engineerObject.GetComponent<Animator>();
            //if (anim != null)
                //anim.SetTrigger("StartDialogue");
        }
    }

    public void SetDialogueToNext()
    {
        if (nextDialogueData != null)
        {
            currentDialogueData = nextDialogueData;
            //Debug.Log("[Engineer] Dialogue changed to next data.");
        }
        else
        {
            //Debug.LogWarning("[Engineer] No nextDialogueData assigned.");
        }
    }

    public void SetDialogueToLoop()
    {
        if (loopDialogueData != null)
        {
            currentDialogueData = loopDialogueData;
            hasEndedDialogue = true; // marcamos que ya terminó
            //Debug.Log("[Engineer] Dialogue changed to loop data and marked as ended.");
        }
        else
        {
            //Debug.LogWarning("[Engineer] No loopDialogueData assigned.");
        }
    }

    public void OnEngineerEndAction()
    {
        SetDialogueToLoop();

        // Activar y desactivar objetos según corresponda
        foreach (var obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
        }

        foreach (var obj in objectsToDeactivate)
        {
            if (obj != null) obj.SetActive(false);
        }

        //Debug.Log("[Engineer] End action executed: Dialogue set to loop and scene objects updated.");
    }

    public NPCData GetNextDialogue() => nextDialogueData;
}
