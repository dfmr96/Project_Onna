using UnityEngine;

public class NPCTriggerAnimation : MonoBehaviour
{
    [Header("Animator del NPC")]
    [SerializeField] private Animator animator;

    [Header("Nombre del trigger/estado al entrar")]
    [SerializeField] private string onEnterAnimation = "Talk";

    [Header("Nombre del trigger/estado al salir")]
    [SerializeField] private string onExitAnimation = "Idle";

    [Header("Usar Triggers en vez de reproducir estados")]
    [SerializeField] private bool useTrigger = true;

    [Header("Tag del objeto que activa el cambio (ej: Player)")]
    [SerializeField] private string activatorTag = "Player";

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(activatorTag)) return;

        if (useTrigger)
            animator.SetTrigger(onEnterAnimation);
        else
            animator.Play(onEnterAnimation);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(activatorTag)) return;

        if (useTrigger)
            animator.SetTrigger(onExitAnimation);
        else
            animator.Play(onExitAnimation);
    }
}
