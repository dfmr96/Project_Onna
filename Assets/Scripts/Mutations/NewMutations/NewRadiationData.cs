using Mutations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRadiationData", menuName = "Mutations/Radiation")]
public class NewRadiationData : ScriptableObject
{
    [SerializeField] private MutationType type;
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite smallIcon;

    public MutationType Type => type;
    public Sprite Icon => icon;
    public Sprite SmallIcon => smallIcon;
}
