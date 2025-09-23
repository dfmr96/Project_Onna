using UnityEngine;
using UnityEngine.UI;
using Mutations;

[RequireComponent(typeof(Button))]
public class NewRadiationButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    private NewRadiationData currentData;
    private SystemType currentSystem;
    private SlotType currentSlot;
    private NewMutationController controller;

    /// <summary>
    /// Initialice button
    /// </summary>
    public void SetupButton(NewRadiationData data, SystemType system, SlotType slot, NewMutationController mutationController)
    {
        currentData = data;
        currentSystem = system;
        currentSlot = slot;
        controller = mutationController;

        if (iconImage != null && currentData != null)
            iconImage.sprite = currentData.Icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    /// <summary>
    /// Callback when the button is pressed
    /// </summary>
    private void OnButtonClicked()
    {
        if (controller == null || currentData == null) 
        {
            Debug.LogError("Something is our of reference");
            return;
        }

        bool success = controller.EquipRadiation(currentData.Type, currentSystem, currentSlot);

        if (success) button.interactable = false;
    }

    /// <summary>
    /// Update radiation
    /// </summary>
    public void UpdateRadiation(NewRadiationData data, SystemType system, SlotType slot) => SetupButton(data, system, slot, controller);
}
