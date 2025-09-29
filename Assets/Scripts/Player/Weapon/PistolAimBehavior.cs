using System;
using UnityEngine;

namespace Player
{
    public class PistolAimBehavior : StateMachineBehaviour
    {
        protected PlayerRigController rigController;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            Debug.Log($"{GetType().Name}: OnStateEnter called - returning to pistol aim");

            if (rigController == null)
            {
                rigController = animator.GetComponent<PlayerRigController>();
            }

            switch (GameModeSelector.SelectedMode)
            {
                case GameMode.Hub:
                    rigController.SetRigState(PlayerRigController.RigState.HUB);
                    break;
                case GameMode.Run:
                    rigController.SetRigState(PlayerRigController.RigState.PistolAim);
                    break;
                case GameMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }
    }
}