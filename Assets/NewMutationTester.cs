using Player.Stats.Runtime;
using UnityEngine;

public class NewMutationTester : MonoBehaviour
{
    [SerializeField] private GameObject mutationCanvasPrefab;
    [SerializeField] private NewMutationDatabase mutationDatabase;

    void Start()
    {
        RunData.Initialize(mutationDatabase);
        Instantiate(mutationCanvasPrefab);
    }
}
