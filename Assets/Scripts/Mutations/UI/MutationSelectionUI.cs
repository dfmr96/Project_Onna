using System.Collections.Generic;
using Mutations.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mutations.UI
{
    public class MutationSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject selectionPanel;
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private GameObject optionPrefab;
        [SerializeField] private Button confirmButton;

        [Header("Services")]
        [SerializeField] private MutationSelectionService selectionService;

        private List<MutationOption> currentOptions;
        private MutationOption selectedOption;
        private List<GameObject> optionButtons = new List<GameObject>();

        private void Start()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmSelection);

            selectionPanel.SetActive(false);
        }

        public void ShowSelectionScreen()
        {
            if (selectionService == null)
            {
                var manager = MutationManager.Instance;
                if (manager != null)
                {
                    selectionService = new MutationSelectionService(manager, manager.GetComponent<RadiationEffectFactory>());
                }
            }

            currentOptions = selectionService.GenerateSelectionOptions(3);
            CreateOptionButtons();
            selectionPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        private void CreateOptionButtons()
        {
            ClearOptionButtons();

            for (int i = 0; i < currentOptions.Count; i++)
            {
                var option = currentOptions[i];
                var buttonObj = Instantiate(optionPrefab, optionsContainer);
                optionButtons.Add(buttonObj);

                var button = buttonObj.GetComponent<Button>();
                var titleText = buttonObj.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                var descText = buttonObj.transform.Find("Description").GetComponent<TextMeshProUGUI>();
                var iconImage = buttonObj.transform.Find("Icon").GetComponent<Image>();
                var levelText = buttonObj.transform.Find("Level").GetComponent<TextMeshProUGUI>();

                titleText.text = $"{option.radiationType} - {option.targetSystem}";
                descText.text = option.description;
                iconImage.sprite = option.icon;
                levelText.text = option.isUpgrade ? $"Level {option.newLevel}" : "New";

                int index = i;
                button.onClick.AddListener(() => SelectOption(index));
            }
        }

        private void SelectOption(int index)
        {
            selectedOption = currentOptions[index];

            for (int i = 0; i < optionButtons.Count; i++)
            {
                var button = optionButtons[i].GetComponent<Button>();
                button.interactable = (i == index);
            }

            if (confirmButton != null)
                confirmButton.interactable = true;
        }

        private void ConfirmSelection()
        {
            if (selectedOption != null && selectionService != null)
            {
                selectionService.ApplySelection(selectedOption);
                CloseSelectionScreen();
            }
        }

        private void CloseSelectionScreen()
        {
            selectionPanel.SetActive(false);
            Time.timeScale = 1f;
            ClearOptionButtons();
            selectedOption = null;
        }

        private void ClearOptionButtons()
        {
            foreach (var button in optionButtons)
            {
                if (button != null)
                    Destroy(button);
            }
            optionButtons.Clear();
        }

        private void OnDestroy()
        {
            if (confirmButton != null)
                confirmButton.onClick.RemoveAllListeners();
        }
    }
}