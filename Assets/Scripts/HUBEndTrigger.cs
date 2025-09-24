using Player;
using Player.Stats.Runtime;
using UnityEngine;

public class HubEndTrigger : InteractableBase
{
    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] private GameObject loadCanvasPrefab;
    [SerializeField] private NewMutationDatabase mutationDatabase;
    public override void Interact()
    {
        base.Interact();
        PlayerHelper.DisableInput();
        SceneManagementUtils.AsyncLoadSceneByName(levelProgression.GetNextRoom(), loadCanvasPrefab, this);
        RunData.Initialize(mutationDatabase);
    }
}
