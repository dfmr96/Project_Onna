using Mutations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMutation", menuName = "NewMutation/Mutation")]
public class NewMutations : ScriptableObject
{
    [Header("Basics")]
    [field: SerializeField] private string mutationName;
    [field: SerializeField] [TextArea] private string description;
    [field: SerializeField] private MutationType type;

    [Header("Effects")]
    [field: SerializeField] private float baseValue;

    [field: SerializeField] private float valueNerviosoMajor;
    [field: SerializeField] private float valueNerviosoMinor;
    [field: SerializeField] private float valueTegumentarioMajor;
    [field: SerializeField] private float valueTegumentarioMinor;
    [field: SerializeField] private float valueMuscularMajor;
    [field: SerializeField] private float valueMuscularMinor;

    public string MutationName => mutationName;
    public string Description => description;
    public MutationType Type => type;

    public float GetValue(SystemType system, SlotType slot)
    {
        return (system, slot) switch
        {
            (SystemType.Nerve, SlotType.Major) => valueNerviosoMajor,
            (SystemType.Nerve, SlotType.Minor) => valueNerviosoMinor,
            (SystemType.Integumentary, SlotType.Major) => valueTegumentarioMajor,
            (SystemType.Integumentary, SlotType.Minor) => valueTegumentarioMinor,
            (SystemType.Muscular, SlotType.Major) => valueMuscularMajor,
            (SystemType.Muscular, SlotType.Minor) => valueMuscularMinor,
            _ => baseValue
        };
    }
}
