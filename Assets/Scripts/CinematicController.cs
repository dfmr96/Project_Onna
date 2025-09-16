using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class CinematicController : MonoBehaviour
{
    [SerializeField] GameObject loadCanvasPrefab;
    [SerializeField] private string nextScene;
    
    [Header("Sprites de la cinemática")]
    [SerializeField] private List<Sprite> cinematicSprites;

    [Header("UI de pantalla")]
    [SerializeField] private Image cinematicImage;

    private int currentIndex = 0;
    
    private void Start()
    {
        if (cinematicSprites == null || cinematicSprites.Count == 0 || cinematicImage == null)
        {
            Debug.LogError("❌ Faltan referencias o sprites en la cinemática.");
            return;
        }

        ShowCurrentSprite();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            AdvanceCinematic();
        }
    }

    private void AdvanceCinematic()
    {
        currentIndex++;
        if (currentIndex > cinematicSprites.Count) return;

        if (currentIndex >= cinematicSprites.Count)
        {
            GameModeSelector.SelectedMode = GameMode.Hub;
            //SceneManagementUtils.AsyncLoadSceneByName(nextScene, loadCanvasPrefab, this);
            SceneManagementUtils.LoadSceneByName(nextScene);
            return;
        }

        ShowCurrentSprite();
    }

    private void ShowCurrentSprite()
    {
        cinematicImage.sprite = cinematicSprites[currentIndex];
    }
}