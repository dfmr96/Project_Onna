using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PlayerRigController : MonoBehaviour
{
    public enum RigState
    {
        PistolAim,
        PistolIdle,
        Melee,
        HUB
    }

    [Header("Rig Layers")]
    public Rig bodyAimRig;
    public Rig weaponPoseRig;
    public Rig weaponAimingRig;
    public Rig weaponMeleeRig;
    public Rig handsIKRig;

    [Header("Animation")]
    public Animator animator;

    private Coroutine weightRoutine;


    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }


    private void TriggerMeleeAnimation()
    {
        SetMeleeRigWeights();

        if (animator != null)
        {
            animator.SetTrigger("meleePerformed");
            Debug.Log("Melee trigger activated");
        }
        else
        {
            Debug.LogWarning("Animator not found!");
        }
    }

    public void SetRigState(RigState state)
    {
        switch (state)
        {
            case RigState.Melee:
                if (mouseAiming != null)
                    mouseAiming.StartMeleeMode();
                SetRigWeights(1f, 0f, 0f, 1f, 0f, "Melee");
                break;
            case RigState.PistolAim:
                if (mouseAiming != null)
                    mouseAiming.EndMeleeMode();
                SetRigWeights(1f, 0f, 1f, 0f, 1f, "Pistol Aim");
                break;
            case RigState.PistolIdle:
                SetRigWeights(1f, 1f, 0f, 1f, 1f, "Pistol Idle");
                break;
            case RigState.HUB:
                SetRigWeights(0,0,0,0,0,"HUB");
                break;
        }
    }

    public void SetMeleeRigWeights()
    {
        SetRigState(RigState.Melee);
    }

    private void SetPistolIdleState()
    {
        SetRigState(RigState.PistolIdle);
    }

    public void SetPistolAimState()
    {
        SetRigState(RigState.PistolAim);
    }

    private void SetRigWeights(float bodyAim, float weaponPose, float weaponAiming, float weaponMelee, float handsIK, string stateName)
    {
        if (weightRoutine != null)
        {
            StopCoroutine(weightRoutine);
        }
        weightRoutine = StartCoroutine(ApplyRigWeights(bodyAim, weaponPose, weaponAiming, weaponMelee, handsIK, stateName));
    }

    private IEnumerator ApplyRigWeights(float bodyAim, float weaponPose, float weaponAiming, float weaponMelee, float handsIK, string stateName)
    {
        yield return null; // Wait one frame to let Animator finish

        if (bodyAimRig != null) bodyAimRig.weight = bodyAim;
        if (weaponPoseRig != null) weaponPoseRig.weight = weaponPose;
        if (weaponAimingRig != null) weaponAimingRig.weight = weaponAiming;
        if (weaponMeleeRig != null) weaponMeleeRig.weight = weaponMelee;
        if (handsIKRig != null) handsIKRig.weight = handsIK;

        currentState = stateName;
        weightRoutine = null;
    }

    [Header("Mouse Aiming")]
    public MouseGroundAiming mouseAiming;

    [Header("Debug")]
    [Space]
    public bool debugRigWeights = false;

    private string currentState = "None";

    private void OnGUI()
    {
        if (!debugRigWeights) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"Current State: {currentState}");
        GUILayout.Space(5);
        GUILayout.Label("Rig Weights:");

        if (bodyAimRig != null)
            GUILayout.Label($"Body Aim: {bodyAimRig.weight:F2}");
        if (weaponPoseRig != null)
            GUILayout.Label($"Weapon Pose: {weaponPoseRig.weight:F2}");
        if (weaponAimingRig != null)
            GUILayout.Label($"Weapon Aiming: {weaponAimingRig.weight:F2}");
        if (weaponMeleeRig != null)
            GUILayout.Label($"Weapon Melee: {weaponMeleeRig.weight:F2}");
        if (handsIKRig != null)
            GUILayout.Label($"Hands IK: {handsIKRig.weight:F2}");

        GUILayout.Space(10);
        GUILayout.Label("Control via PlayerRigController methods:");

        GUILayout.EndArea();
    }
}