using UnityEngine;

namespace Mutations
{
    [CreateAssetMenu(fileName = "New Mutation", menuName = "Mutations/Mutation", order = 0)]
    public class MutationData : ScriptableObject
    {
        [field:SerializeField] private string mutationName;
        [field:SerializeField] [TextArea]  private string description;
        [field:SerializeField] private OrganType organ;
        [field:SerializeField] private Sprite icon;
        [field:SerializeField] private UpgradeEffect upgradeEffect;
        public string MutationName
        {
            get => mutationName;
            set => mutationName = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public OrganType Organ
        {
            get => organ;
            set => organ = value;
        }

        public Sprite Icon => icon;

        public UpgradeEffect UpgradeEffect => upgradeEffect;
    }
}