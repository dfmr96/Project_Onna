using Player;
using Player.Stats;
using Player.Stats.Meta;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mutations.Testing
{
    public class MetaUpgradeUI : MonoBehaviour
    {
        [SerializeField] private PlayerModel player;
        [SerializeField] private StatReferences statRefs;

        [Header("Botones")]
        [SerializeField] private Button speedButton;
        [SerializeField] private Button healthButton;
        [SerializeField] private Button damageButton;

        private void Start()
        {
            if (player == null) player = FindObjectOfType<PlayerModel>();

            speedButton.onClick.AddListener(() => ApplyFlat(statRefs.movementSpeed, 1f));
            healthButton.onClick.AddListener(() => ApplyFlat(statRefs.maxVitalTime, 5f));
            damageButton.onClick.AddListener(() => ApplyPercent(statRefs.damage, 10f));
        }

        private void ApplyFlat(StatDefinition stat, float amount)
        {
            player.StatContext.Meta?.AddFlatBonus(stat, amount);
        }

        private void ApplyPercent(StatDefinition stat, float percent)
        {
            player.StatContext.Meta?.AddPercentBonus(stat, percent);
        }
        
        public void ResetMetaStats()
        {
            MetaStatSaveSystem.DeleteSaveFile();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}