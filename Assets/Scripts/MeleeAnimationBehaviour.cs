using UnityEngine;

public class MeleeAnimationBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("MeleeAnimationBehaviour: OnStateExit called");

        RigLayerTestScript rigController = animator.GetComponent<RigLayerTestScript>();
        if (rigController != null)
        {
            Debug.Log("MeleeAnimationBehaviour: Found RigLayerTestScript, calling SetPistolAimState");
            rigController.SetPistolAimState();
        }
        else
        {
            Debug.LogWarning("MeleeAnimationBehaviour: RigLayerTestScript not found!");
        }
    }
}