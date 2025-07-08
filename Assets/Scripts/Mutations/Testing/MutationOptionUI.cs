using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mutations.Testing
{
    public class MutationOptionUI : MonoBehaviour
    {
        [SerializeField] private Image mutationIcon;
        [SerializeField] private TextMeshProUGUI mutationName;
        [SerializeField] private TextMeshProUGUI mutationDescription;
        [SerializeField] private Button selectButton;
        
        private MutationData mutation;

        public void SetData(MutationData mutationData)
        {
            mutation = mutationData;

            if (mutationIcon != null)
                mutationIcon.sprite = mutation.Icon;

            if (mutationName != null)
                mutationName.text = mutation.MutationName;

            if (mutationDescription != null)
                mutationDescription.text = mutation.Description;

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnSelected);
            }
        }
        
        public void OnSelected()
        {
            var playerStats = PlayerHelper.GetPlayer().GetComponent<PlayerModel>().StatContext.Runtime;
            if (playerStats == null)
            {
                Debug.LogWarning("⛔ No RuntimeStats available.");
                return;
            }

            mutation.UpgradeEffect.Apply(playerStats);
            Debug.Log($"✅ Mutación aplicada: {mutation.MutationName}");

            transform.parent.gameObject.SetActive(false);
        }
    }
}