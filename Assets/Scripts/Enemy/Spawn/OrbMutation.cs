using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OrbMutation : InteractableBase
{

    [SerializeField] private GameObject mutationCanvasPrefab;
    [ContextMenu("Show mutation selection")]
    private void ShowMutationSelection() => Instantiate(mutationCanvasPrefab);

    public override void Interact()
    {
        base.Interact();
        ShowMutationSelection();
    
        // Notificar al GameManager para manejar la secuencia completa
        GameManager.Instance?.OnOrbMutationActivated(this);
    }


}
