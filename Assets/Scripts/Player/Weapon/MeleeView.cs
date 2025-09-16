using UnityEngine;

public class MeleeView : MonoBehaviour
{
    [Header("Animation References")]
    [SerializeField] private Animator animator;
    [Header("Internal References")]
    [SerializeField] private MeleeController meleeController;
    private MeleeModel model => meleeController.Model;

    public void Initialize() => model.OnAttackStep += HandleAttackStarted;

    private void OnDestroy() => model.OnAttackStep -= HandleAttackStarted;

    private void HandleAttackStarted(int comboStep)
    {
        switch (comboStep){
            case 1:
                animator.SetTrigger("Attack1");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
            default:
                break;
        }
    }

}
