using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private HubManager hub;
    [SerializeField] private NPCData data;
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6) DialogueManager.Instance.StartDialogue(data);
    }
}
