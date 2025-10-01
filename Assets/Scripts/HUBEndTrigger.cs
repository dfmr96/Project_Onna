using Player;
using Player.Stats.Runtime;
using UnityEngine;

public class HubEndTrigger : InteractableBase
{
    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] private GameObject loadCanvasPrefab;
    [SerializeField] private MutationDB mutationDB;
    public override void Interact()
    {
        base.Interact();
        PlayerHelper.DisableInput();
        SceneManagementUtils.AsyncLoadSceneByName(levelProgression.GetNextRoom(), loadCanvasPrefab, this);
    }
}
