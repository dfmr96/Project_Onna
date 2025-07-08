using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private HubManager hub;
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
            hub.OpenStore();
    }
}
