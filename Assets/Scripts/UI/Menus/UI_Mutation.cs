using Mutations;
using Player.Stats.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mutation : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform systemPanelParent;
    [SerializeField] private Transform radiationPanelParent;
    [SerializeField] private GameObject radiationButtonPrefab;

    [Header("Images")]
    [SerializeField] private Image targetImage;

    [Header("Animators")]
    [SerializeField] private Animator mutationAnimator;
    [SerializeField] private Animator radiationAnimator;

    [Header("Rotation")]
    [SerializeField] private float rotationStep = 120f;
    [SerializeField] private float rotationSpeed = 200f;

    private bool isRotating = false;
    private float currentRotation = 0f;

    private NewMutationController mController;

    private SystemType activeSystem = SystemType.Nerve;
    private List<NewRadiationData> currentRolledRadiations = new();

    private void Start()
    {
        mController = RunData.NewMutationController;
        if (mController == null)
        {
            Debug.LogError("NewMutationController was not finded");
            return;
        }
        StartCoroutine(InitialSequence());
    }

    private IEnumerator InitialSequence()
    {
        radiationAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(GetAnimationLength(radiationAnimator, "Open"));

        mutationAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(GetAnimationLength(mutationAnimator, "Open"));

        Initialize();
    }

    private void Initialize()
    {
        currentRolledRadiations = mController.RollRadiations();
        UpdateRadiationUI();
    }

    public void OnNextSystem()
    {
        if (!isRotating)
        {
            activeSystem = activeSystem switch
            {
                SystemType.Nerve => SystemType.Integumentary,
                SystemType.Integumentary => SystemType.Muscular,
                SystemType.Muscular => SystemType.Nerve,
                _ => SystemType.Nerve
            };

            StartCoroutine(RotateAndSetSystem());
        }
    }

    private IEnumerator RotateAndSetSystem()
    {
        yield return RotateSequence();
        UpdateSystemUI();
    }

    private void UpdateSystemUI()
    {
        NewMutationSystem systemData = mController.GetSystem(activeSystem);

        // Mostrar slots mayor y menor
    }

    private void UpdateRadiationUI()
    {
        Debug.Log("Updating radiation UI");

        foreach (var radData in currentRolledRadiations)
        {
            GameObject btnObj = Instantiate(radiationButtonPrefab, radiationPanelParent);
            NewRadiationButton radButton = btnObj.GetComponent<NewRadiationButton>();

            radButton.SetupButton(radData, activeSystem, SlotType.Major, mController);
        }
    }

    public void RotateWithAnimation()
    {
        if (!isRotating) StartCoroutine(RotateSequence());
    }

    private IEnumerator RotateSequence()
    {
        isRotating = true;

        // 1. MutationClose
        mutationAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(GetAnimationLength(mutationAnimator, "Close"));

        // 2. Radiation entra en Block
        radiationAnimator.SetTrigger("Block");

        // 3. Rueda rota
        currentRotation += rotationStep;
        yield return StartCoroutine(RotateTo(currentRotation));

        // 4. MutationOpen
        mutationAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(GetAnimationLength(mutationAnimator, "Open"));

        // 5. Radiation Unblock
        radiationAnimator.SetTrigger("Unblock");
        yield return new WaitForSeconds(GetAnimationLength(radiationAnimator, "Unblock"));

        // 6. Radiation Idle
        radiationAnimator.SetTrigger("Idle");

        isRotating = false;
    }

    private IEnumerator RotateTo(float targetAngle)
    {
        float startAngle = targetImage.rectTransform.eulerAngles.z;
        float angle = startAngle;

        while (Mathf.Abs(Mathf.DeltaAngle(angle, targetAngle)) > 0.1f)
        {
            angle = Mathf.MoveTowardsAngle(angle, targetAngle, rotationSpeed * Time.deltaTime);
            targetImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        targetImage.rectTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private float GetAnimationLength(Animator animator, string stateName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == stateName)
                return clip.length;
        }
        return 0.5f;
    }
}
