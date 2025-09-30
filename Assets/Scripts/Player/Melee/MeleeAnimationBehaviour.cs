using UnityEngine;

namespace Player.Melee
{
    public class MeleeAnimationBehaviour : StateMachineBehaviour
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("MeleeAnimationBehaviour: OnStateEnter called");
        
            MeleeController meleeController = animator.GetComponent<MeleeController>();
            if (meleeController != null)
            {
                Debug.Log("MeleeAnimationBehaviour: Executing damage");
                //meleeController.ExecuteDamage();
            }
            else
            {
                Debug.LogWarning("MeleeAnimationBehaviour: MeleeController not found!");
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("MeleeAnimationBehaviour: OnStateExit called");

            MeleeController meleeController = animator.GetComponent<MeleeController>();
            if (meleeController != null)
            {
                Debug.Log("MeleeAnimationBehaviour: Animation complete");
                //meleeController.OnAnimationComplete();
                //meleeController.DisableComboWindow();
            }
            else
            {
                Debug.LogWarning("MeleeAnimationBehaviour: MeleeController not found!");
            }
        }
    }
}