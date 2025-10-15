using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Trigger Name")]
    [SerializeField] private string startAnimation;

    [Header("Use_Trigger")]
    [SerializeField] private bool useTrigger = true;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (!string.IsNullOrEmpty(startAnimation))
        {
            if (useTrigger)
                animator.SetTrigger(startAnimation);
            else
                animator.Play(startAnimation);
        }
    }

    public void PlayAnimation(string animationName, bool asTrigger = true)
    {
        if (asTrigger)
            animator.SetTrigger(animationName);
        else
            animator.Play(animationName);
    }
}
