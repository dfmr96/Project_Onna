using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image npcImage;
    [SerializeField] private Button[] optionButtons;

    public void SetName(string name) { nameText.text = name; }

    public void SetImage(Sprite image) { npcImage.sprite = image; }

    public void BindActions(DialogueNode node)
    {
        foreach (var option in node.Options)
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

    public void DisplayNode(DialogueNode node, System.Action<int> onOptionSelected)
    {
        dialogueText.text = node.DialogueText;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < node.Options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                int index = i;
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.Options[i].optionText;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => onOptionSelected(index));
            }
            else optionButtons[i].gameObject.SetActive(false);
        }
    }
}
