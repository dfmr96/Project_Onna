using Mutations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRadiationData", menuName = "Mutations/Radiation")]
public class NewRadiationData : ScriptableObject
{
    [SerializeField] private MutationType type;
    [SerializeField] private Sprite icon;

    public MutationType Type => type;
    public Sprite Icon => icon;
}
