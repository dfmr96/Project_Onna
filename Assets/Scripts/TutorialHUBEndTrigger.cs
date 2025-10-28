using Player;
using Player.Stats.Runtime;
using UnityEngine;

public class TutorialHUBEndTrigger : InteractableBase
{
    [SerializeField] private LevelProgression levelProgression;
    [SerializeField] private GameObject loadCanvasPrefab;
    public override void Interact()
    {
        base.Interact();
        PlayerHelper.DisableInput();
        SaveSystem.MarkTutorialDone();
        SceneManagementUtils.AsyncLoadSceneByName(levelProgression.GetNextRoom(), loadCanvasPrefab, this);
    }
}
