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
    private SlotType currentSlot;
    private NewMutationController controller;
    private UI_Mutation ui;

    /// <summary>
    /// Initialice button
    /// </summary>
    public void SetupButton(NewRadiationData data, SlotType slot, NewMutationController mutationController, UI_Mutation uiMutation)
    {
        currentData = data;
        currentSlot = slot;
        controller = mutationController;
        ui = uiMutation;

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

        bool success = controller.EquipRadiation(currentData.Type, ui.ActiveSystem, currentSlot);

        if (success) button.interactable = false;
    }

    /// <summary>
    /// Update radiation
    /// </summary>
    public void UpdateRadiation(NewRadiationData data, SystemType system, SlotType slot, UI_Mutation uiMutation) => SetupButton(data, slot, controller, uiMutation);
}
