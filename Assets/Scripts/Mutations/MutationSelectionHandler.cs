using System;
using System.Collections;
using System.Collections.Generic;
using Mutations.Testing;
using NaughtyAttributes;
using Player;
using UnityEngine;

namespace Mutations
{
    public class MutationSelectionHandler : MonoBehaviour
    {
        [SerializeField] private MutationSelector selector;
        [SerializeField] private MutationOptionUI mutationUIPrefab;
        [SerializeField] private Transform uiContainer;
        [SerializeField] private CanvasGroup canvasGroup;

        private readonly List<MutationOptionUI> activeUIs = new List<MutationOptionUI>();
        private PlayerModel _playerModel;
        private void Start()
        {
            
            PlayerHelper.DisableInput();
            _playerModel = PlayerHelper.GetPlayer().GetComponent<PlayerModel>();
            _playerModel.EnablePassiveDrain(false);
            if (selector == null || mutationUIPrefab == null || uiContainer == null)
            {
                Debug.LogError("❌ Faltan referencias en MutationSelectionHandler.");
                return;
            }
            
            Time.timeScale = 0.1f;

            RollAndDisplayMutations();
    
            StartCoroutine(EnableUIAfterDelay(3f));
        }

        private IEnumerator EnableUIAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            Time.timeScale = 1f;

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            float duration = 0.5f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            foreach (var ui in activeUIs)
                ui.SetInteractable(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RollAndDisplayMutations();
            }
        }

        [Button("Roll Mutations and Show UI")]
        private void RollAndDisplayMutations()
        {
            // Limpiar lista y hijos
            foreach (Transform child in uiContainer)
                Destroy(child.gameObject);
            activeUIs.Clear();

            var mutations = selector.RollMutations(3);
            Debug.Log("Mutations rolled successfully.");

            foreach (var mutation in mutations)
            {
                var ui = Instantiate(mutationUIPrefab, uiContainer);
                ui.SetData(mutation);
                ui.SetCloseUI(CloseUI);
                ui.SetInteractable(false); 
                activeUIs.Add(ui);
            }

            gameObject.SetActive(true);
            Debug.Log("Showing UI for mutation selection.");
        }

        private void CloseUI()
        {
            PlayerHelper.EnableInput();
            _playerModel.EnablePassiveDrain(true);
            gameObject.SetActive(false); 
        }
    }
}
