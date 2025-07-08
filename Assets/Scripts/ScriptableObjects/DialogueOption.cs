using System;

[Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueNode nextNode;
    public bool endsDialogue;

    public DialogueActionId actionId;

    [NonSerialized]
    public Action onSelectedAction;
}