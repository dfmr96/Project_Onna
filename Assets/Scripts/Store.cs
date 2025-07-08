using UnityEngine;

public class Store : InteractableBase
{
    [SerializeField] private HubManager hub;
    [SerializeField] private NPCData data;

    public override void Interact() 
    { 
        base.Interact();
        DialogueManager.Instance.StartDialogue(data);
    }
}
