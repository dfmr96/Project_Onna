using UnityEngine;

public class DefeatUIController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ArrangeLetters arrangeLetters; // referencia al otro script

    private void Awake()
    {
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void OnIntroAnimationFinished()
    {
        if (arrangeLetters != null)
        {
            arrangeLetters.ShowAndFillLetters();
        }
    }
}
