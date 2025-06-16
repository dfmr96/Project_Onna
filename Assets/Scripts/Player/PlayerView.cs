using System;
using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform aimTarget; 
        [SerializeField] private Transform torsoTransform; 
        [SerializeField] private Transform weaponSocket;   
        [SerializeField] private GameObject weaponInstance; 
        [SerializeField] private Animator animator;
        [SerializeField] private Transform visualRoot;
        
        [Header("Params")]
        [SerializeField] private float torsoRotationSpeed = 10f;
        [SerializeField] private float rotationSpeed = 10f;
        
        private PlayerController _playerController;
        private PlayerInputHandler _playerInputHandler;
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Speed = Animator.StringToHash("Speed");

        private void RotateVisualTowards(Vector3 moveDir)
        {
            if (moveDir.sqrMagnitude < 0.01f) return;

            moveDir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            visualRoot.rotation = Quaternion.Slerp(
                visualRoot.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }

        private void Awake()
        {
            if (weaponInstance != null && weaponSocket != null)
            {
                weaponInstance.transform.SetParent(weaponSocket, false);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
            }

            _playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Update()
        {
            animator.SetFloat(Speed, _playerController.Speed);
            Vector3 moveDir = _playerInputHandler.MovementInput;

            animator.SetFloat(MoveX, moveDir.x);
            animator.SetFloat(MoveY, moveDir.z);

            RotateVisualTowards(moveDir);
            
            UpdateAimTarget(_playerController.MouseWorldPos);     
            UpdateWeaponAim(_playerController.MouseWorldPos);   
        }

        public void UpdateAimTarget(Vector3 worldPos)
        {
            if (aimTarget == null || torsoTransform == null) return;

            worldPos.y = torsoTransform.position.y;
            aimTarget.position = worldPos;

            Vector3 direction = worldPos - torsoTransform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                torsoTransform.rotation = Quaternion.Slerp(torsoTransform.rotation, targetRot, Time.deltaTime * torsoRotationSpeed);
            }
        }

        public void UpdateWeaponAim(Vector3 targetPos)
        {
            if (weaponInstance == null) return;

            Vector3 lookAt = targetPos;
            lookAt.y = weaponInstance.transform.position.y;

            weaponInstance.transform.LookAt(lookAt);
        }
        
        public void SetPlayerController(PlayerController controller)
        {
            _playerController = controller;
        }
        
        private void OnDrawGizmos()
        {
            if (visualRoot != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(visualRoot.position, visualRoot.position + visualRoot.forward * 2f);
            }
        }
    }
}