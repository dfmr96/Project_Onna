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
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(ammoUI.InitBulletsDelayed());
    }

    private void OnEnable()
    {
        PlayerModel.OnUpdateTime += timeUI.UpdateTimeUI;
        WeaponController.OnShoot += ammoUI.UpdateBulletsLeft;
    }

    private void OnDisable()
    {
        PlayerModel.OnUpdateTime -= timeUI.UpdateTimeUI;
        WeaponController.OnShoot -= ammoUI.UpdateBulletsLeft;
    }
}
