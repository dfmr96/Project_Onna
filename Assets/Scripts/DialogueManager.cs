using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject dialoguePrefab;
    private GameObject dialogueInstance;
    private DialogueNode currentNode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); por ahora solo funciona en el hub pero
        //cuando haya dialogos en todo el juego esto NO DEBERIA destruirse!
    }

    public void StartDialogue(NPCData npcData)
    {
        //Cursor Mouse
        Cursor.visible = true;

        if (dialogueInstance == null) dialogueInstance = Instantiate(dialoguePrefab);

        dialogueInstance.gameObject.SetActive(true);
        dialogueInstance.GetComponent<DialogueUI>().SetName(npcData.NpcName);
        dialogueInstance.GetComponent<DialogueUI>().SetImage(npcData.NpcImage);
        dialogueInstance.GetComponent<DialogueUI>().BindActions(npcData.StartingDialogue);
        ShowNode(npcData.StartingDialogue);

        PlayerHelper.DisableInput();
    }

    void ShowNode(DialogueNode node)
    {
        currentNode = node;
        dialogueInstance.GetComponent<DialogueUI>().DisplayNode(node, OnOptionSelected);
    }

    void OnOptionSelected(int index)
    {
        var selectedOption = currentNode.Options[index];
        selectedOption.onSelectedAction?.Invoke();

        if (selectedOption.endsDialogue)
        {
            EndDialogue();
            return;
        }

        if (selectedOption.nextNode != null)
        {
            dialogueInstance.GetComponent<DialogueUI>().BindActions(selectedOption.nextNode);
            ShowNode(selectedOption.nextNode);
        }
        else EndDialogue();
    }

    void EndDialogue()
    {
        dialogueInstance.gameObject.SetActive(false);
        currentNode = null;
        PlayerHelper.EnableInput();

        //Cursor Mouse
        Cursor.visible = false;
    }
}
