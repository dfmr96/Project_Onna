using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea]
    [SerializeField] private string dialogueText;
    [SerializeField] private DialogueOption[] options;

    public string DialogueText => dialogueText;
    public DialogueOption[] Options => options;
}
