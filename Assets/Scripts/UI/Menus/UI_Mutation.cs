using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mutation : MonoBehaviour
{
    [Header("Imagen que rota")]
    public Image targetImage;

    [Header("Animators")]
    public Animator mutationAnimator;
    public Animator radiationAnimator;

    [Header("Rotación")]
    public float rotationStep = 120f;
    public float rotationSpeed = 200f;

    private bool isRotating = false;
    private float currentRotation = 0f;

    void Start()
    {
        StartCoroutine(InitialSequence());
    }

    private IEnumerator InitialSequence()
    {
        // Radiation abre
        radiationAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(GetAnimationLength(radiationAnimator, "Open"));

        // Mutation cierra
        mutationAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(GetAnimationLength(mutationAnimator, "Open"));
    }

    public void RotateWithAnimation()
    {
        if (!isRotating)
            StartCoroutine(RotateSequence());
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
        return 0.5f; // fallback si no encuentra
    }
}
