using UnityEngine;
using Player;
using Player.Weapon;

public class UI_Player_Manager : MonoBehaviour
{
    public static UI_Player_Manager Instance;

    [Header("Submódulos de UI")]
    [SerializeField] private UI_TimeCircles timeUI;
    [SerializeField] private UI_Ammo ammoUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;

        // Autodescubrir refs si no están asignadas en Inspector
        if (timeUI == null)
            timeUI = GetComponentInChildren<UI_TimeCircles>(true);

        if (ammoUI == null)
            ammoUI = GetComponentInChildren<UI_Ammo>(true);
    }

    private void Start()
    {
        if (ammoUI != null)
            StartCoroutine(ammoUI.InitBulletsDelayed());
        else
            Debug.LogError("ammoUI no está asignado en UI_Player_Manager!");
    }

    private void OnEnable()
    {
        if (timeUI != null)
            PlayerModel.OnUpdateTime += timeUI.UpdateTimeUI;

        if (ammoUI != null)
            WeaponController.OnShoot += ammoUI.UpdateBulletsLeft;
    }

    private void OnDisable()
    {
        PlayerModel.OnUpdateTime -= timeUI.UpdateTimeUI;
        WeaponController.OnShoot -= ammoUI.UpdateBulletsLeft;
    }

    private void OnDestroy()
    {
        // Limpieza extra por si OnDisable no se llama en reload de escena
        PlayerModel.OnUpdateTime -= timeUI.UpdateTimeUI;
        WeaponController.OnShoot -= ammoUI.UpdateBulletsLeft;
    }
}
