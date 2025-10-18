using System;
using UnityEngine;

[Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueNode nextNode;
    public bool endsDialogue;
    
    // Ahora puede tener múltiples acciones
    public DialogueActionId[] actionIds;

    [NonSerialized]
    public Action onSelectedAction;
}
