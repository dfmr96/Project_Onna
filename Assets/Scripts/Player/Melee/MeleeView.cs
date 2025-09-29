using UnityEngine;

namespace Player.Melee
{
    public class MeleeView : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private MeleeController meleeController;

        private static readonly int ComboStepParam = Animator.StringToHash("ComboStep");

        private void OnEnable()
        {
            meleeController.OnComboStepChanged += HandleComboStep;
            meleeController.OnComboReset += HandleComboReset;
        }

        private void OnDisable()
        {
            meleeController.OnComboStepChanged -= HandleComboStep;
            meleeController.OnComboReset -= HandleComboReset;
        }

        private void HandleComboStep(int step)
        {
            animator.SetInteger(ComboStepParam, step);
        }

        private void HandleComboReset()
        {
            animator.SetInteger(ComboStepParam, 0);
        }
    }
}
