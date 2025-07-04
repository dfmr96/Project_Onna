using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Player;
using UnityEditor.Experimental.GraphView;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Button[] optionButtons;

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
        BindActions(npcData.startingDialogue);
        ShowNode(npcData.startingDialogue);
        nameText.text = npcData.npcName;
        PlayerHelper.DisableInput();
    }

    void BindActions(DialogueNode node)
    {
        foreach (var option in node.options)
        {
            option.onSelectedAction = null;

            switch (option.actionId)
            {
                case DialogueActionId.OpenStore:
                    option.onSelectedAction = () =>
                    {
                        HubManager hub = HubManager.Instance;
                        if (hub != null) hub.OpenStore();
                    };
                    break;

                case DialogueActionId.None:
                default:
                    break;
            }
        }
    }

    void ShowNode(DialogueNode node)
    {
        currentNode = node;
        dialoguePanel.SetActive(true);
        dialogueText.text = node.dialogueText;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < node.options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                int index = i;
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.options[i].optionText;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
            }
            else optionButtons[i].gameObject.SetActive(false);
        }
    }

    void OnOptionSelected(int index)
    {
        var selectedOption = currentNode.options[index];

        selectedOption.onSelectedAction?.Invoke();

        if (selectedOption.endsDialogue)
        {
            EndDialogue();
            return;
        }

        if (selectedOption.nextNode != null)
        {
            BindActions(selectedOption.nextNode);
            ShowNode(selectedOption.nextNode);
        }
        else EndDialogue();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentNode = null;
        PlayerHelper.EnableInput();
    }
}
