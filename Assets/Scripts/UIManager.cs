using Player;
using Player.Weapon;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private UIData data;
    [SerializeField] private Image timeCircle;
    [SerializeField] private Image timeWeaponCooldown;
    private float targetCooldownFill;
    private float fillSpeed = 2f;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }
    private void OnEnable() { PlayerModel.OnUpdateTime += UpdateTimeUI; WeaponController.OnShoot += UpdateWeaponCooldown; }
    private void OnDisable() { PlayerModel.OnUpdateTime -= UpdateTimeUI; WeaponController.OnShoot += UpdateWeaponCooldown; }
    private void Update()
    {
        if (timeWeaponCooldown != null)
        {
            timeWeaponCooldown.fillAmount = Mathf.Lerp(
                timeWeaponCooldown.fillAmount,
                targetCooldownFill,
                Time.deltaTime * fillSpeed
            );
        }
    }
    private void UpdateTimeUI(float timePercent) 
    {
        timeCircle.fillAmount = timePercent;
        //if (timePercent < data.TimeToHurry)
        //{
        //    float pulse = 1 + Mathf.Sin(Time.time * 10f) * 0.05f;
        //    timeCircle.transform.localScale = new Vector3(pulse, pulse, 1);
        //    timeCircle.color = data.HurryColor;
        //}
        //else 
        //{
        //    timeCircle.transform.localScale = Vector3.one;
        //    timeCircle.color = data.NormalColor;
        //}
    }

    private void UpdateWeaponCooldown(int actualAmmo, int totalAmmo) { targetCooldownFill = 1f - (float)actualAmmo / totalAmmo; }
}
