using Player;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactPrefab;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 2f, 0);
    [SerializeField] private AudioClip interactAudio;

    protected GameObject interactPrompt;
    protected Transform playerPos;

    protected virtual void Start()
    {
        playerPos = PlayerHelper.GetPlayer().transform;
        if (interactPrompt == null)
        {
            interactPrompt = Instantiate(interactPrefab, transform.position + promptOffset, Quaternion.identity);
            interactPrompt.transform.SetParent(transform);
        }
        interactPrompt.SetActive(false);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
            other.GetComponent<PlayerController>().ToggleInteraction(true);
        }
    }

    public virtual void Interact() => AudioManager.Instance?.PlaySFX(interactAudio);
}
