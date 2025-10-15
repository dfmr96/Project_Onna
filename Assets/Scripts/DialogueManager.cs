using UnityEngine;
using Player;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private DialogueUI dialogueUI;
    private DialogueNode currentNode;
    private NPCData currentNPCData;
    public InteractableBase CurrentTrigger { get; private set; }

    [SerializeField] private DialogueUI dialogueUIPrefab; // prefab de UI

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(NPCData npcData, InteractableBase trigger = null)
    {
        if (npcData == null || npcData.StartingDialogue == null)
        {
            //Debug.LogError("StartDialogue: npcData o StartingDialogue es null!");
            return;
        }

        CurrentTrigger = trigger;
        currentNPCData = npcData;
        currentNode = npcData.StartingDialogue;

        // Mostrar cursor
        CursorHelper.Show();

        // Instanciar prefab si aún no existe
        if (dialogueUI == null)
        {
            dialogueUI = Instantiate(dialogueUIPrefab);
        }

        dialogueUI.gameObject.SetActive(true);
        dialogueUI.SetName(npcData.NpcName);
        dialogueUI.SetImage(npcData.NpcImage);
        dialogueUI.BindActions(currentNode);
        dialogueUI.DisplayNode(currentNode, OnOptionSelected);

        // Desactivar control del jugador
        PlayerHelper.DisableInput();
    }

    private void OnOptionSelected(int index)
    {
        var option = currentNode.Options[index];

        // Ejecuta acción asociada (incluye ChangeDialogue, OpenStore, etc.)
        option.onSelectedAction?.Invoke();

        if (option.endsDialogue)
        {
            EndDialogue();
            return;
        }

        if (option.nextNode != null)
        {
            currentNode = option.nextNode;
            dialogueUI.BindActions(currentNode);
            dialogueUI.DisplayNode(currentNode, OnOptionSelected);
        }
    }

    public void EndDialogue()
    {
        if (dialogueUI != null)
            dialogueUI.gameObject.SetActive(false);

        CurrentTrigger = null;
        currentNode = null;

        // Reactivar input del jugador
        PlayerHelper.EnableInput();

        // Ocultar cursor solo si no hay otra UI activa (por ejemplo tienda)
        if (!HubManager.Instance || !HubManager.Instance.IsStoreOpen)
            CursorHelper.Hide();
    }

    public void SetCurrentNPCData(NPCData newData)
    {
        currentNPCData = newData;
    }
}
