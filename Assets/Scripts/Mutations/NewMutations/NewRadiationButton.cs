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
    public void SetupButton(NewRadiationData data, NewMutationController mutationController, UI_Mutation uiMutation)
    {
        currentData = data;
        controller = mutationController;
        ui = uiMutation;
        Debug.Log("Button setted");

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
        if (ui == null || currentData == null)
        {
            Debug.LogError("Something is out of reference");
            return;
        }

        Debug.Log("Button clicked");
        ui.OnRadiationSelected(currentData);
    }

    /// <summary>
    /// Update radiation
    /// </summary>
    public void UpdateRadiation(NewRadiationData data, SystemType system, SlotType slot, UI_Mutation uiMutation) => SetupButton(data, controller, uiMutation);
}
