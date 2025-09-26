using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

namespace Mutations
{
    //[CreateAssetMenu(fileName = "New Mutation Database", menuName = "Mutations/Database", order = 0)]
    public class MutationDatabase : ScriptableObject
    {
        [SerializeField] private List<MutationData> allMutations;
        [SerializeField] private SerializedDictionary <OrganType, List<MutationData>> mutationLookup;
        
        public IEnumerable<OrganType> AvailableOrgans => mutationLookup.Keys;
        public List<MutationData> GetMutationsByOrgan(OrganType organ)
        {
            return mutationLookup.TryGetValue(organ, out var list)
                ? list
                : new List<MutationData>();
        }
        
        [Button("Rebuild Lookup")]
        private void RebuildLookup()
        {
            mutationLookup = new SerializedDictionary<OrganType, List<MutationData>>();

            foreach (var mutation in allMutations)
            {
                if (!mutationLookup.TryGetValue(mutation.Organ, out var list))
                {
                    list = new List<MutationData>();
                    mutationLookup[mutation.Organ] = list;
                }

                list.Add(mutation);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            Debug.Log("Mutation lookup rebuilt and saved.");
        }
    }
}
