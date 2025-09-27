using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PlayerRigController : MonoBehaviour
{
    [Header("Rig Layers")]
    public Rig bodyAimRig;
    public Rig weaponPoseRig;
    public Rig weaponAimingRig;
    public Rig weaponMeleeRig;
    public Rig handsIKRig;

    [Header("Animation")]
    public Animator animator;


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

    private void SetMeleeRigWeights()
    {
        if (mouseAiming != null)
            mouseAiming.StartMeleeMode();

        StartCoroutine(ForceRigWeights(1f, 0f, 0f, 1f, 0f, "Melee"));
    }

    private void SetPistolIdleState()
    {
        StartCoroutine(ForceRigWeights(1f, 1f, 0f, 1f, 1f, "Pistol Idle"));
    }

    public void SetPistolAimState()
    {
        if (mouseAiming != null)
            mouseAiming.EndMeleeMode();

        //Debug.Log("SetPistolAimState called - Starting coroutine to force rig weights...");
        StartCoroutine(ForceRigWeights(1f, 0f, 1f, 0f, 1f, "Pistol Aim"));
    }

    private IEnumerator ForceRigWeights(float bodyAim, float weaponPose, float weaponAiming, float weaponMelee,
        float handsIK, string stateName)
    {
        for (int i = 0; i < 10; i++)
        {
            if (bodyAimRig != null) bodyAimRig.weight = bodyAim;
            if (weaponPoseRig != null) weaponPoseRig.weight = weaponPose;
            if (weaponAimingRig != null) weaponAimingRig.weight = weaponAiming;
            if (weaponMeleeRig != null) weaponMeleeRig.weight = weaponMelee;
            if (handsIKRig != null) handsIKRig.weight = handsIK;

            yield return new WaitForEndOfFrame();
        }

        currentState = stateName;
        /*Debug.Log($"{stateName} state forced - Final weights:");
        if (bodyAimRig != null) Debug.Log($"Body Aim: {bodyAimRig.weight}");
        if (weaponPoseRig != null) Debug.Log($"Weapon Pose: {weaponPoseRig.weight}");
        if (weaponAimingRig != null) Debug.Log($"Weapon Aiming: {weaponAimingRig.weight}");
        if (weaponMeleeRig != null) Debug.Log($"Weapon Melee: {weaponMeleeRig.weight}");
        if (handsIKRig != null) Debug.Log($"Hands IK: {handsIKRig.weight}");
        */
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