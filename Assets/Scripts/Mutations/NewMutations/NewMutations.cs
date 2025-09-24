using Mutations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMutation", menuName = "NewMutation/Mutation")]
public class NewMutations : ScriptableObject
{
    [Header("Basics")]
    [field: SerializeField] [TextArea] private string majorEffectDescription;
    [field: SerializeField] [TextArea] private string minorEffectDescription;
    [field: SerializeField] private MutationType type;

    public string MajorEffectDescription => majorEffectDescription;
    public string MinorEffectDescription => minorEffectDescription;
    public MutationType Type => type;
}
