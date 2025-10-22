using NaughtyAttributes;
using Player.Stats.Runtime;
using UnityEngine;

public class MutationDebugRunner : MonoBehaviour
{
    [SerializeField] NewMutationController controller;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            controller = RunData.NewMutationController;
            PrintDebug();
        }
    }
    
    [Button("Print Mutation Debug Info (F10)")]
    private void PrintDebug()
    {
        if (controller == null)
        {
            Debug.LogWarning("⚠️ controller is null.");
            return;
        }

        controller.DebugPrintStatus();
    }
    
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            controller = RunData.NewMutationController;
    }
#endif
}
