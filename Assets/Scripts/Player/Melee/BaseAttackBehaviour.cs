using UnityEngine;

namespace Player.Melee
{
    public abstract class BaseAttackBehaviour : StateMachineBehaviour
    {
        protected MeleeController MeleeController;
        private PlayerRigController _rigController;

        public void SetMeleeController(MeleeController controller)
        {
            MeleeController = controller;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log($"{GetType().Name}: OnStateEnter called");

            if (_rigController == null)
            {
                _rigController = animator.GetComponent<PlayerRigController>();
            }
            _rigController.SetRigState(PlayerRigController.RigState.Melee);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log($"{GetType().Name}: OnStateExit called");

            if (MeleeController != null)
            {
                Debug.Log($"{GetType().Name}: Animation complete");
                MeleeController.OnAnimationComplete();
            }
        }
    }
}