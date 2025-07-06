using Player;
using Player.Weapon;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private UIData data;
    [SerializeField] private Image timeCircle;
    [SerializeField] private Image weaponOverheat;
    [SerializeField] private Image weaponCooling;
    
    
    private CooldownSettings _coolingSettings;
    private float targetCooldownFill;
    private float fillSpeed = 2f;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerModel.OnUpdateTime += UpdateTimeUI; 
        WeaponController.OnShoot += UpdateWeaponCooldown;
        WeaponController.OnCooling += UpdateCoolingCooldown;
    }

    private void OnDisable()
    {
        PlayerModel.OnUpdateTime -= UpdateTimeUI; 
        WeaponController.OnShoot -= UpdateWeaponCooldown;
        WeaponController.OnCooling -= UpdateCoolingCooldown;
    }
    private void Update()
    {
        if (weaponOverheat != null)
        {
            weaponOverheat.fillAmount = Mathf.Lerp(
                weaponOverheat.fillAmount,
                targetCooldownFill,
                Time.deltaTime * fillSpeed
            );
        }
    }
    private void UpdateTimeUI(float timePercent) 
    {
        timeCircle.fillAmount = timePercent;
    }

    private void UpdateWeaponCooldown(int actualAmmo, int totalAmmo)
    {
        targetCooldownFill = 1f - (float)actualAmmo / totalAmmo;
    }

    private void UpdateCoolingCooldown(float coolingTimer, float coolingCooldown)
    {
        //Debug.Log(coolingTimer + "/" + coolingCooldown);
        weaponCooling.fillAmount = 1f - (coolingTimer / coolingCooldown);
        
        /*if (_coolingSettings == null)
        {
            _coolingSettings = coolingSettings;
            Debug.Log("Cooling settings set");
        }
        
        float coolingCooldown = _coolingSettings.CoolingCooldown;
        float coolingTimer = 0;
        
        Debug.Log("Pre Cooling timer: " + coolingTimer);
        while (coolingTimer < coolingCooldown)
        {
            coolingTimer += Time.deltaTime;
            weaponCooling.fillAmount = 1f - (coolingTimer / coolingCooldown);
            
        }
        weaponCooling.fillAmount = 0;
        Debug.Log("Post Cooling timer: " + coolingTimer);*/
    }
}
