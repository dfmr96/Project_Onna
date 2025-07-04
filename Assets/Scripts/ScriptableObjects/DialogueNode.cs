using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    [TextArea]
    public string dialogueText;

    public DialogueOption[] options;
}
