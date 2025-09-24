using Mutations;
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

        Initialize();
    }

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
        ShowSlotSelection(radData);
        ShowSlotDescriptions(radData);
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
        NewMutations majorMutation = mController.GetMutationForSlot(data.Type, activeSystem, SlotType.Major);
        NewMutations minorMutation = mController.GetMutationForSlot(data.Type, activeSystem, SlotType.Minor);

        majorSlotDescription.text = majorMutation != null ? majorMutation.MajorEffectDescription : "";
        majorSlotDescription.color = normalTextColor;

        minorSlotDescription.text = minorMutation != null ? minorMutation.MinorEffectDescription : "";
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
            majorSlotDescription.text = equippedMajorMutation != null ? equippedMajorMutation.MajorEffectDescription : "";
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
            minorSlotDescription.text = equippedMinorMutation != null ? equippedMinorMutation.MinorEffectDescription : "";
            minorSlotDescription.color = equippedTextColor;
        }
        else minorSlotDescription.text = "";
    }

    private void TryEquip(NewRadiationData radData, SlotType slot)
    {
        bool equipped = mController.EquipRadiation(radData.Type, activeSystem, slot);
        if (equipped) 
        {
            UpdateSystemUI();
            OnRadiationEquipped();
        }
    }

    private IEnumerator RotateAndSetSystem()
    {
        DestroySlotDescriptions();
        yield return RotateSequence();
        UpdateSystemUI();
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
