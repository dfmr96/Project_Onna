using UnityEngine;

namespace Player
{
    public class PlayerWeaponView : MonoBehaviour
    {
        [Header("Weapon References")] 
        [SerializeField] private GameObject weaponInstance;
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private Transform aimTarget;
        [SerializeField] private Transform torsoTransform;

        [Header("Weapon Parameters")] 
        [SerializeField] private float minAimDistance = 2f;
        [SerializeField] private float torsoRotationSpeed = 10f;

        private bool _weaponVisible = true;

        public void Initialize()
        {
            // Configurar arma inicial si existe
            if (weaponInstance != null && weaponSocket != null)
            {
                weaponInstance.transform.SetParent(weaponSocket, false);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
            }
        }

        public void SetWeaponVisibility(bool visible)
        {
            Debug.Log($"🔫 PlayerWeaponView.SetWeaponVisibility called with: {visible} (current: {_weaponVisible})");

            if (_weaponVisible == visible)
            {
                Debug.Log("🔫 Weapon visibility already matches, skipping update");
                return;
            }

            _weaponVisible = visible;

            if (weaponInstance != null)
            {
                Debug.Log($"🔫 Setting weaponInstance.SetActive({visible})");
                weaponInstance.SetActive(visible);
            }
            else
            {
                Debug.LogWarning("🔫 weaponInstance is null!");
            }
        }

        public void UpdateAiming(Vector3 mouseWorldPos)
        {
            if (!_weaponVisible) return;

            UpdateAimTarget(mouseWorldPos);
            UpdateWeaponAim(mouseWorldPos);
        }

        private void UpdateAimTarget(Vector3 worldPos)
        {
            if (aimTarget == null || torsoTransform == null) return;

            // Mantener la altura del torso
            worldPos.y = torsoTransform.position.y;

            Vector3 direction = worldPos - torsoTransform.position;
            direction.y = 0f;

            float sqrDist = direction.sqrMagnitude;

            // Si está demasiado cerca, reubicarlo a una distancia mínima
            if (sqrDist < minAimDistance * minAimDistance)
            {
                direction = direction.normalized * minAimDistance;
                worldPos = torsoTransform.position + direction;
            }

            aimTarget.position = worldPos;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                torsoTransform.rotation = Quaternion.Slerp(
                    torsoTransform.rotation,
                    targetRot,
                    Time.deltaTime * torsoRotationSpeed
                );
            }
        }

        private void UpdateWeaponAim(Vector3 targetPos)
        {
            if (weaponInstance == null || torsoTransform == null) return;

            Vector3 origin = torsoTransform.position; // usar el torso como centro
            Vector3 dir = targetPos - origin;
            dir.y = 0f;

            float sqrDist = dir.sqrMagnitude;

            // Si está muy cerca, reubicar
            if (sqrDist < minAimDistance * minAimDistance)
            {
                dir = dir.normalized * minAimDistance;
                targetPos = origin + dir;
            }

            // Mantener la altura del arma
            targetPos.y = weaponInstance.transform.position.y;

            weaponInstance.transform.LookAt(targetPos);
        }

        public bool IsWeaponVisible()
        {
            return _weaponVisible;
        }

        private void OnDrawGizmos()
        {
            if (torsoTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(torsoTransform.position, minAimDistance);
            }
        }
    }
}