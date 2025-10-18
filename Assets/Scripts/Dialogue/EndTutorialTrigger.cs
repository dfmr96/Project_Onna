using System.Collections.Generic;
using UnityEngine;

public class EndTutorialTrigger : InteractableBase
{
    [Header("Datos del diálogo final del tutorial")]
    [SerializeField] private NPCData endTutorialDialogue;

    [Header("GameObject que se activará al finalizar el tutorial")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
     
    [Header("Triggers del tutorial a desactivar")]
    [SerializeField] private Collider[] triggersToDisable;

    [Header("Action ID que indica el fin del tutorial")]
    [SerializeField] private string actionId = "EndTutorial";

    private bool hasTriggered = false;

    protected override void Start() { /* no queremos instanciar nada aquí */ }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        // Deshabilitamos el collider para no reactivarlo
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
            col.enabled = false;

        // Iniciamos el diálogo final
        DialogueManager.Instance.StartDialogue(endTutorialDialogue, this);
    }

    public void HandleAction(string action)
    {
        if (action == actionId)
        {

            foreach (var go in triggersToDisable)
            {
                if (go != null)
                {
                    go.enabled = false;
                    Debug.Log($"Desactivado trigger: {go.name}");
                }
            }

            foreach (var obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"Activado objeto: {obj.name}");
                }
            }

        }
    }

    public NPCData GetDialogueData() => endTutorialDialogue;

    public void SetDialogueData(NPCData newData)
    {
        endTutorialDialogue = newData;
    }
}
