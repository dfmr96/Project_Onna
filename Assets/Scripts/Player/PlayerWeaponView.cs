using UnityEngine;

namespace Player
{
    public class PlayerWeaponView : MonoBehaviour
    {
        [Header("Weapon References")] 
        [SerializeField] private GameObject weaponInstance;
        

        public void SetWeaponVisibility(bool visible)
        {
            if (weaponInstance != null)
            {
                //Debug.Log($"🔫 Setting weaponInstance.SetActive({visible})");
                weaponInstance.SetActive(visible);
            }
            else
            {
                Debug.LogWarning("🔫 weaponInstance is null!");
            }
        }
    }
}