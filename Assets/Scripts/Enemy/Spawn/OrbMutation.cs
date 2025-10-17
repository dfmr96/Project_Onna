using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class OrbMutation : InteractableBase
{

    [SerializeField] private GameObject mutationCanvasPrefab;
    [ContextMenu("Show mutation selection")]
    private void ShowMutationSelection() => Instantiate(mutationCanvasPrefab);


    //private void OnTriggerEnter(Collider other)
    //{
    //     //Interfaz para Mutaciones
    //    var collector = other.GetComponent<IOrbCollectable>();
    //    if (collector != null)
    //    {
    //        ShowMutationSelection();
    //        Destroy(gameObject);
    //    }


    //}

    public override void Interact()
    {
        base.Interact();
        ShowMutationSelection();
        Destroy(gameObject);
    }

}
