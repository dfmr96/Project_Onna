using Player.Stats.Runtime;
using UnityEngine;

public class NewMutationTester : MonoBehaviour
{
    [SerializeField] private GameObject mutationCanvasPrefab;

    void Start()
    {
        RunData.Initialize();
        Instantiate(mutationCanvasPrefab);
    }
}
