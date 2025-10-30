using Mutations;
using Mutations.Core;
using Player;
using Player.Stats.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [Header("System Buttons")]
    [SerializeField] private Button nerveButton;
    [SerializeField] private Button integumentaryButton;
    [SerializeField] private Button muscularButton;
    [SerializeField] private Animator systemAnimator1;
    [SerializeField] private Animator systemAnimator2;

    [Header("Hover Tooltip")]
    [SerializeField] private TextMeshProUGUI hoverText;
    [SerializeField] private CanvasGroup hoverCanvasGroup; 

    [SerializeField] private AudioClip openCanvasSfx;






    [Header("Pulse Effect (Alpha)")]
    [SerializeField] private float pulseDuration = 1f;  // Tiempo para subir/bajar alpha
    [SerializeField] private float pulseDelay = 0f;     // Pausa entre alternancias
    [SerializeField] private float pulseMinAlpha = 0.2f; // Nuevo rango mínimo
    [SerializeField] private float pulseMaxAlpha = 1f;   // Rango máximo (pleno)
    [SerializeField] private GameObject majorPulseObject;
    [SerializeField] private GameObject minorPulseObject;


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

        if (openCanvasSfx != null)
            PlaySound(openCanvasSfx);

        nerveButton.onClick.AddListener(() => OnSelectSystem(SystemType.Nerve));
        integumentaryButton.onClick.AddListener(() => OnSelectSystem(SystemType.Integumentary));
        muscularButton.onClick.AddListener(() => OnSelectSystem(SystemType.Muscular));

        UpdateSystemButtons();

        systemAnimator2.SetTrigger("Muscular");
        systemAnimator1.SetTrigger("Integumentary");

        SetupButtonHover(nerveButton, SystemType.Nerve);
        SetupButtonHover(integumentaryButton, SystemType.Integumentary);
        SetupButtonHover(muscularButton, SystemType.Muscular);


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
    
    private void SetPulseObjectActive(bool majorActive, bool minorActive)
    {
        if (majorPulseObject != null)
            majorPulseObject.SetActive(majorActive);

        if (minorPulseObject != null)
            minorPulseObject.SetActive(minorActive);
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

        // Detenemos cualquier pulso previo
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        bool majorOccupied = mController.GetEquippedRadiationData(activeSystem, SlotType.Major) != null;
        bool minorOccupied = mController.GetEquippedRadiationData(activeSystem, SlotType.Minor) != null;

        if (!majorOccupied)
            majorImg.sprite = radData.SmallIcon;
        if (!minorOccupied)
            minorImg.sprite = radData.SmallIcon;

        Color cMajor = majorImg.color;
        Color cMinor = minorImg.color;
        cMajor.a = 1f;
        cMinor.a = 1f;
        majorImg.color = cMajor;
        minorImg.color = cMinor;

        // Activar objetos de pulso
        SetPulseObjectActive(true, true);

        // Iniciar corutina de pulso solo en los slots libres
        if (!majorOccupied && !minorOccupied)
            pulseRoutine = StartCoroutine(PulseAlternate(majorImg, minorImg, majorPulseObject, minorPulseObject));
        else if (!majorOccupied)
            pulseRoutine = StartCoroutine(PulseSingle(majorImg, majorPulseObject));
        else if (!minorOccupied)
            pulseRoutine = StartCoroutine(PulseSingle(minorImg, minorPulseObject));
        else
            SetPulseObjectActive(false, false); // Ninguno libre → desactivar objetos
    }


    private IEnumerator PulseSingle(Image img, GameObject pulseObj)
    {
        Color c = img.color;
        c.a = 0.5f; // inicio en 0.5
        img.color = c;

        while (true)
        {
            float t = 0f;

            // Fade in
            while (t < 1f)
            {
                t += Time.deltaTime / pulseDuration;
                c.a = Mathf.SmoothStep(0.5f, 1f, t);
                img.color = c;

                if (pulseObj != null)
                    pulseObj.SetActive(true);

                yield return null;
            }

            // Fade out
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / pulseDuration;
                c.a = Mathf.SmoothStep(1f, 0.5f, t);
                img.color = c;

                if (pulseObj != null)
                    pulseObj.SetActive(false);

                yield return null;
            }

            yield return new WaitForSeconds(pulseDelay);
        }
    }

    private IEnumerator PulseAlternate(Image majorImg, Image minorImg, GameObject majorObj, GameObject minorObj)
    {
        bool majorActive = true;

        Color cMajor = majorImg.color;
        Color cMinor = minorImg.color;

        cMajor.a = 0.5f;
        cMinor.a = 0.5f;
        majorImg.color = cMajor;
        minorImg.color = cMinor;

        while (true)
        {
            float t = 0f;

            // Fade in
            while (t < 1f)
            {
                t += Time.deltaTime / pulseDuration;
                float alpha = Mathf.SmoothStep(0.5f, 1f, t);

                if (majorActive)
                {
                    cMajor.a = alpha;
                    majorImg.color = cMajor;
                    if (majorObj != null) majorObj.SetActive(true);

                    cMinor.a = 0.5f;
                    minorImg.color = cMinor;
                    if (minorObj != null) minorObj.SetActive(false);
                }
                else
                {
                    cMinor.a = alpha;
                    minorImg.color = cMinor;
                    if (minorObj != null) minorObj.SetActive(true);

                    cMajor.a = 0.5f;
                    majorImg.color = cMajor;
                    if (majorObj != null) majorObj.SetActive(false);
                }

                yield return null;
            }

            // Fade out
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / pulseDuration;
                float alpha = Mathf.SmoothStep(1f, 0.5f, t);

                if (majorActive)
                {
                    cMajor.a = alpha;
                    majorImg.color = cMajor;
                    if (majorObj != null) majorObj.SetActive(false);
                }
                else
                {
                    cMinor.a = alpha;
                    minorImg.color = cMinor;
                    if (minorObj != null) minorObj.SetActive(false);
                }

                yield return null;
            }

            yield return new WaitForSeconds(pulseDelay);

            majorActive = !majorActive;
        }
    }



    private void OnSelectSystem(SystemType targetSystem)
    {
        if (isRotating || targetSystem == activeSystem)
            return;

        activeSystem = targetSystem;
        HideHoverTooltip();
        StartCoroutine(RotateAndSetSystem());
    }

    private void UpdateSystemButtons()
    {
        SetButtonState(nerveButton, activeSystem != SystemType.Nerve);
        SetButtonState(integumentaryButton, activeSystem != SystemType.Integumentary);
        SetButtonState(muscularButton, activeSystem != SystemType.Muscular);
    }

    private void SetButtonState(Button button, bool active)
    {
        button.interactable = active;
        var img = button.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = active ? 1f : 0.3f; // un poco apagado si no se puede
            img.color = c;
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

        // Estado de ocupación
        bool majorOccupied = mController.GetEquippedRadiationData(activeSystem, SlotType.Major) != null;
        bool minorOccupied = mController.GetEquippedRadiationData(activeSystem, SlotType.Minor) != null;

        // Mutaciones correspondientes a esta radiación
        RadiationEffect minorMutation = mController.GetMutationForSlot(data.Type, activeSystem, SlotType.Minor);
        RadiationEffect majorMutation = mController.GetMutationForSlot(data.Type, activeSystem, SlotType.Major);

        // Solo actualizamos el texto de los slots vacíos
        if (!majorOccupied && majorMutation != null)
        {
            majorSlotDescription.text = majorMutation.Description;
            majorSlotDescription.color = normalTextColor;
        }

        if (!minorOccupied && minorMutation != null)
        {
            minorSlotDescription.text = minorMutation.Description;
            minorSlotDescription.color = normalTextColor;
        }
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
            // Detener pulso y objetos
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }
            SetPulseObjectActive(false, false);

            // Ajustar alpha de botones a 1
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
        isRotating = true;
        nerveButton.interactable = false;
        integumentaryButton.interactable = false;
        muscularButton.interactable = false;
        SetRadiationButtonsInteractable(false);

        ResetRadiationUI();
        UpdateSystemButtons();
        DestroySlotDescriptions();

        // Desactivar objetos durante rotación
        SetPulseObjectActive(false, false);

        yield return RotateSequence();
        UpdateSystemUI();

        // Reactivar slots si hay radiación disponible
        if (currentSelectedRad != null)
        {
            ShowSlotDescriptions(currentSelectedRad);
            ShowSlotIconsPreview(currentSelectedRad); // aquí se volverán a activar los objetos
        }

        nerveButton.interactable = true;
        integumentaryButton.interactable = true;
        muscularButton.interactable = true;
        SetRadiationButtonsInteractable(true);

        UpdateSystemButtons();
        isRotating = false;
    }



    private void SetRadiationButtonsInteractable(bool interactable)
    {
        foreach (Transform child in radiationPanelParent)
        {
            Button b = child.GetComponent<Button>();
            if (b != null)
                b.interactable = interactable;
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
        systemAnimator2.gameObject.SetActive(false);
        systemAnimator1.gameObject.SetActive(false);
        nerveButton.interactable = false;
        integumentaryButton.interactable = false;
        muscularButton.interactable = false;

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

        radiationAnimator.SetTrigger("Idle");
        systemAnimator2.gameObject.SetActive(true);
        systemAnimator1.gameObject.SetActive(true);
        yield return StartCoroutine(PlayOtherSystemAnimations());

        nerveButton.interactable = true;
        integumentaryButton.interactable = true;
        muscularButton.interactable = true;
        UpdateSystemButtons();

        UpdateSystemButtons();

        isRotating = false;
    }


    private IEnumerator PlayOtherSystemAnimations()
    {
        if (systemAnimator1 == null || systemAnimator2 == null)
            yield break;

        // Lista de todos los sistemas
        List<SystemType> allSystems = new() { SystemType.Nerve, SystemType.Integumentary, SystemType.Muscular };

        // Quitamos el activo
        allSystems.Remove(activeSystem);

        // Asignamos a cada Animator un sistema restante
        SystemType sys1 = allSystems[0];
        SystemType sys2 = allSystems[1];

        string trigger1 = sys1.ToString();
        string trigger2 = sys2.ToString();

        systemAnimator1.SetTrigger(trigger1);
        systemAnimator2.SetTrigger(trigger2);

        // Esperamos la duración más larga de los 2 clips
        float animLength1 = GetAnimationLength(systemAnimator1, trigger1);
        float animLength2 = GetAnimationLength(systemAnimator2, trigger2);

        yield return new WaitForSeconds(Mathf.Max(animLength1, animLength2));
    }

    private void ShowHover(SystemType system)
    {
        if (hoverText == null || hoverCanvasGroup == null) return;

        hoverText.text = system.ToString();
        hoverCanvasGroup.alpha = 1f;
    }

    private void HideHover()
    {
        if (hoverCanvasGroup == null) return;
        hoverCanvasGroup.alpha = 0f;
    }

    private void HideHoverTooltip()
    {
        if (hoverCanvasGroup != null)
            hoverCanvasGroup.alpha = 0f;
    }


    private void SetupButtonHover(Button button, SystemType system)
    {
        var trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        trigger.triggers.Clear();

        // Pointer Enter
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) =>
        {
            if (button.interactable) ShowHover(system);
        });
        trigger.triggers.Add(entryEnter);

        // Pointer Exit
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => HideHover());
        trigger.triggers.Add(entryExit);
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

    public void PlaySound(AudioClip audioClip) => AudioManager.Instance?.PlaySFX(audioClip);
}
