using UnityEngine;

namespace Player.Melee
{
    public class FirstAttackBehaviour : BaseAttackBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (MeleeController != null)
            {
                MeleeController.StartComboWindow(stateInfo.length);
            }
        }
    }
}