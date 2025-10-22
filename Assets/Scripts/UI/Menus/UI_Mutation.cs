using Mutations;
using Mutations.Core;
using Player;
using Player.Stats.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mutation : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI systemName;
    [SerializeField] private Transform radiationPanelParent;
    [SerializeField] private GameObject radiationButtonPrefab;
    [SerializeField] private Button majorSlotButton;
    [SerializeField] private Button minorSlotButton;
    [SerializeField] private TextMeshProUGUI majorSlotDescription;
    [SerializeField] private TextMeshProUGUI minorSlotDescription;

    [Header("Text colors")]
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color equippedTextColor = new Color(0.7f, 0.7f, 0.7f);

    [Header("Images && Sprites")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite emptyMajorSlotSprite;
    [SerializeField] private Sprite emptyMinorSlotSprite;

    [Header("Animators")]
    [SerializeField] private Animator mutationAnimator;
    [SerializeField] private Animator radiationAnimator;

    [Header("Rotation")]
    [SerializeField] private float rotationStep = 120f;
    [SerializeField] private float rotationSpeed = 200f;

    [Header("Pulse Effect (Alpha)")]
    [SerializeField] private float pulseDuration = 0.8f;  // Tiempo para subir/bajar alpha
    [SerializeField] private float pulseDelay = 0.2f;     // Pausa entre alternancias

    private Coroutine pulseRoutine;
    private NewRadiationData currentSelectedRad;
    

    private bool isRotating = false;
    private float currentRotation = 0f;

    private NewMutationController mController;
    private PlayerModel _playerModel;

    private SystemType activeSystem = SystemType.Nerve;
    private List<NewRadiationData> currentRolledRadiations = new();

    public SystemType ActiveSystem => activeSystem;

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

        Cursor.visible = true;

        Initialize();
    }

    [ContextMenu("Re Roll")]
    private void Initialize()
    {
        PlayerHelper.DisableInput();
        _playerModel = PlayerHelper.GetPlayer().GetComponent<PlayerModel>();
        _playerModel.EnablePassiveDrain(false);

        currentRolledRadiations = mController.RollRadiations();
        UpdateRadiationUI();
        UpdateSystemUI();
        UpdateSystemText();
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

    public void OnRadiationSelected(NewRadiationData radData)
    {
        if (isRotating) return;

        currentSelectedRad = radData; // Guardamos la radiación seleccionada

        ShowSlotSelection(radData);
        ShowSlotDescriptions(radData);
        ShowSlotIconsPreview(radData);
    }


    private void ShowSlotIconsPreview(NewRadiationData radData)
    {
        if (radData == null) return;

        Image majorImg = majorSlotButton.GetComponent<Image>();
        Image minorImg = minorSlotButton.GetComponent<Image>();

        majorImg.sprite = radData.SmallIcon;
        minorImg.sprite = radData.SmallIcon;

        // Detener pulso anterior
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        // Iniciar pulso alternado
        pulseRoutine = StartCoroutine(PulseAlternate(majorImg, minorImg));
    }

    private IEnumerator PulseAlternate(Image majorImg, Image minorImg)
    {
        bool majorActive = true;

        Color cMajor = majorImg.color;
        Color cMinor = minorImg.color;

        // Inicializamos ambos en alpha 0
        cMajor.a = 0f;
        cMinor.a = 0f;
        majorImg.color = cMajor;
        minorImg.color = cMinor;

        while (true)
        {
            float t = 0f;

            // 🔆 Fade in
            while (t < 1f)
            {
                t += Time.deltaTime / pulseDuration;
                float alpha = Mathf.SmoothStep(0f, 1f, t);

                if (majorActive)
                {
                    cMajor.a = alpha;
                    majorImg.color = cMajor;
                    cMinor.a = 0f;
                    minorImg.color = cMinor;
                }
                else
                {
                    cMinor.a = alpha;
                    minorImg.color = cMinor;
                    cMajor.a = 0f;
                    majorImg.color = cMajor;
                }

                yield return null;
            }

            // 🔅 Fade out
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / pulseDuration;
                float alpha = Mathf.SmoothStep(1f, 0f, t);

                if (majorActive)
                {
                    cMajor.a = alpha;
                    majorImg.color = cMajor;
                }
                else
                {
                    cMinor.a = alpha;
                    minorImg.color = cMinor;
                }

                yield return null;
            }

            yield return new WaitForSeconds(pulseDelay);

            majorActive = !majorActive;
        }
    }



    private void ShowSlotSelection(NewRadiationData radData)
    {
        majorSlotButton.onClick.RemoveAllListeners();
        majorSlotButton.onClick.AddListener(() =>
        {
            TryEquip(radData, SlotType.Major);
        });

        minorSlotButton.onClick.RemoveAllListeners();
        minorSlotButton.onClick.AddListener(() =>
        {
            TryEquip(radData, SlotType.Minor);
        });
    }

    private void ShowSlotDescriptions(NewRadiationData data)
    {
        if (isRotating) return;
        RadiationEffect minorMutation = mController.GetMutationForSlot(data.Type, activeSystem, SlotType.Minor);
        RadiationEffect majorMutation = mController.GetMutationForSlot(data.Type, activeSystem, SlotType.Major);

        majorSlotDescription.text = majorMutation != null ? majorMutation.Description : "";
        majorSlotDescription.color = normalTextColor;

        minorSlotDescription.text = minorMutation != null ? minorMutation.Description : "";
        minorSlotDescription.color = normalTextColor;
    }

    private void DestroySlotDescriptions()
    {
        var majorRad = mController.GetEquippedRadiationData(activeSystem, SlotType.Major);
        if (majorRad != null)
        {
            var equippedMajorMutation = mController.GetMutationForSlot(
                majorRad.Type,
                activeSystem,
                SlotType.Major
            );
            majorSlotDescription.text = equippedMajorMutation != null ? equippedMajorMutation.Description : "";
            majorSlotDescription.color = equippedTextColor;
        }
        else majorSlotDescription.text = "";

        var minorRad = mController.GetEquippedRadiationData(activeSystem, SlotType.Minor);
        if (minorRad != null)
        {
            var equippedMinorMutation = mController.GetMutationForSlot(
                minorRad.Type,
                activeSystem,
                SlotType.Minor
            );
            minorSlotDescription.text = equippedMinorMutation != null ? equippedMinorMutation.Description : "";
            minorSlotDescription.color = equippedTextColor;
        }
        else minorSlotDescription.text = "";
    }

    private void TryEquip(NewRadiationData radData, SlotType slot)
    {
        bool equipped = mController.EquipRadiation(radData.Type, activeSystem, slot);
        if (equipped)
        {
            // Detener pulso
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }

            // Aseguramos que ambos slots queden visibles al 100%
            Image majorImg = majorSlotButton.GetComponent<Image>();
            Image minorImg = minorSlotButton.GetComponent<Image>();
            Color cMajor = majorImg.color;
            Color cMinor = minorImg.color;
            cMajor.a = 1f;
            cMinor.a = 1f;
            majorImg.color = cMajor;
            minorImg.color = cMinor;

            majorSlotButton.enabled = false;
            minorSlotButton.enabled = false;

            UpdateSystemUI();
            OnRadiationEquipped();
        }
    }

    private IEnumerator RotateAndSetSystem()
    {
        // Detener cualquier pulso y limpiar UI
        ResetRadiationUI();

        DestroySlotDescriptions();

        yield return RotateSequence();
        UpdateSystemUI(); // Slots con radiaciones equipadas

        // 🔹 Reaplicar información de la radiación seleccionada automáticamente
        if (currentSelectedRad != null)
        {
            ShowSlotDescriptions(currentSelectedRad);
            ShowSlotIconsPreview(currentSelectedRad);
        }
    }

    private void ResetRadiationUI()
    {
        // Detener el pulso si existe
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        // Reiniciar alpha de los slots
        Image majorImg = majorSlotButton.GetComponent<Image>();
        Image minorImg = minorSlotButton.GetComponent<Image>();

        Color cMajor = majorImg.color;
        Color cMinor = minorImg.color;
        cMajor.a = 1f;
        cMinor.a = 1f;

        majorImg.color = cMajor;
        minorImg.color = cMinor;
    }



    private void UpdateSystemUI()
    {
        UpdateSystemText();
        var majorRad = mController.GetEquippedRadiationData(activeSystem, SlotType.Major);
        if (majorRad != null)
            majorSlotButton.GetComponent<Image>().sprite = majorRad.SmallIcon;
        else
            majorSlotButton.GetComponent<Image>().sprite = emptyMajorSlotSprite;

        var minorRad = mController.GetEquippedRadiationData(activeSystem, SlotType.Minor);
        if (minorRad != null)
            minorSlotButton.GetComponent<Image>().sprite = minorRad.SmallIcon;
        else
            minorSlotButton.GetComponent<Image>().sprite = emptyMinorSlotSprite;
    }

    private void UpdateSystemText() => systemName.text = activeSystem.ToString();

    private void UpdateRadiationUI()
    {
        foreach (Transform child in radiationPanelParent)
            Destroy(child.gameObject);

        foreach (var radData in currentRolledRadiations)
        {
            GameObject btnObj = Instantiate(radiationButtonPrefab, radiationPanelParent);
            NewRadiationButton radButton = btnObj.GetComponent<NewRadiationButton>();

            radButton.SetupButton(radData, mController, this);
        }
    }

    public void RotateWithAnimation()
    {
        if (!isRotating) OnNextSystem();
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

    public void OnRadiationEquipped()
    {
        isRotating = true;
        DestroySlotDescriptions();
        foreach (Transform child in radiationPanelParent)
        {
            Button b = child.GetComponent<Button>();
            if (b != null) b.interactable = false;
        }
        StartCoroutine(CloseAfterDelay(2f));
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        DestroySlotDescriptions();
        mutationAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(GetAnimationLength(mutationAnimator, "Close"));
        PlayerHelper.EnableInput();
        Cursor.visible = false;

        Destroy(gameObject);
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
