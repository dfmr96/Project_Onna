using AYellowpaper.SerializedCollections;
using Mutations;
using Mutations.Core;
using UnityEngine;

[System.Serializable]
public struct SystemsInfo
{
    [SerializeField] private MutationSlots mutationsSlots;
}

[System.Serializable]
public struct RadiationInfo
{
    [SerializeField] private SerializedDictionary<MutationType, SystemsInfo> radiationLookup;
}

[System.Serializable]
public struct MutationSlots
{
    [SerializeField] private ScriptableObject majorSO;
    [SerializeField] private ScriptableObject minorSO;
    public MutationSlots(ScriptableObject majorSO, ScriptableObject minorSO)
    {
        this.majorSO = majorSO;
        this.minorSO = minorSO;
    }
}

[CreateAssetMenu(fileName = "New Mutation Database", menuName = "Mutations/Databases")]
public class NewMutationDatabase : ScriptableObject
{
    [Header("Database Mutations")]
    [SerializeField] private SerializedDictionary<SystemType, RadiationInfo> radiationLookup;
}
