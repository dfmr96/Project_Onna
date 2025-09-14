using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Player
{
    public class PlayerView_OLD : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform aimTarget;
        [SerializeField] private Transform torsoTransform;
        [SerializeField] private Transform weaponSocket;
        [SerializeField] private GameObject weaponInstance;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private AudioClip hurtFx;
        [SerializeField] private AudioClip healthFx;

        [Header("Animation Systems")] [SerializeField]
        private RigBuilder rigBuilder;

        [SerializeField] private RuntimeAnimatorController hubAnimator;
        [SerializeField] private RuntimeAnimatorController combatAnimator;

        [Header("Params")] [SerializeField] private float torsoRotationSpeed = 10f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float minAimDistance = 2f;

        private PlayerController _playerController;
        private PlayerInputHandler _playerInputHandler;
        private bool _isInHub => GameModeSelector.SelectedMode == GameMode.Hub;
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsInHub = Animator.StringToHash("IsInHub");

        private AudioSource audioSource;

        //Efecto Visual de Dano
        private Color flashColor = Color.white;
        private float flashDuration = 0.1f;
        private Renderer[] renderers;
        private Color[][] originalColors;
        private readonly string[] colorPropertyNames = { "_BaseColor", "_Color", "_MainColor" };


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

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            renderers = GetComponentsInChildren<Renderer>();
            originalColors = new Color[renderers.Length][];

            for (int i = 0; i < renderers.Length; i++)
            {
                var mats = renderers[i].materials;
                originalColors[i] = new Color[mats.Length];

                for (int j = 0; j < mats.Length; j++)
                {
                    var mat = mats[j];
                    string property = GetColorProperty(mat);

                    if (!string.IsNullOrEmpty(property))
                        originalColors[i][j] = mat.GetColor(property);
                }
            }

            UpdateWeaponVisibility();
            SetAnimatorForCurrentMode();
            UpdateRigBuilderState();
        }

        private void Update()
        {
            Vector3 moveDir = _playerInputHandler.MovementInput;

            if (_isInHub)
            {
                UpdateHubMovement(moveDir);
            }
            else
            {
                UpdateAimTarget(_playerController.MouseWorldPos);
                UpdateWeaponAim(_playerController.MouseWorldPos);
                RotateVisualTowards(moveDir);
            }
        }

        private void UpdateHubMovement(Vector3 moveDir)
        {
            float speed = moveDir.magnitude;

            if (speed < 0.01f)
            {
                // Idle en HUB
                animator.SetFloat(Speed, 0f);
                animator.SetFloat(MoveX, 0f);
                animator.SetFloat(MoveY, 0f);
                return;
            }

            // En el HUB: orientación simple hacia donde se mueve
            moveDir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            visualRoot.rotation = Quaternion.Slerp(
                visualRoot.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            // Animación simple: solo hacia adelante cuando se mueve
            animator.SetFloat(Speed, speed);
            animator.SetFloat(MoveX, 0f);
            animator.SetFloat(MoveY, 1f); // Siempre corriendo hacia adelante
        }

        private void RotateVisualTowards(Vector3 moveDir)
        {
            if (moveDir.sqrMagnitude < 0.01f)
            {
                animator.SetFloat(Speed, 0f);
                animator.SetFloat(MoveX, 0f);
                animator.SetFloat(MoveY, 0f);
                return;
            }

            moveDir.y = 0;
            Vector3 aimDir = GetAimDirection();

            // Calcular producto punto para determinar relación entre movimiento y apuntado
            float dotProduct = Vector3.Dot(moveDir.normalized, aimDir.normalized);

            Vector3 legsDirection;
            Vector3 relativeMovement;

            // Escenario 1: Movimiento hacia adelante (dot > 0.7) - Las piernas miran hacia donde apunta
            if (dotProduct > 0.7f)
            {
                legsDirection = aimDir;
                relativeMovement = Vector3.forward; // Animación hacia adelante
            }
            // Escenario 3: Movimiento hacia atrás (dot < -0.7) - Las piernas miran hacia donde apunta, pero camina hacia atrás
            else if (dotProduct < -0.7f)
            {
                legsDirection = aimDir;
                relativeMovement = Vector3.back; // Animación hacia atrás
            }
            // Escenario 2: Movimiento lateral/strafe - Las piernas miran hacia donde apunta, movimiento lateral
            else
            {
                legsDirection = aimDir;
                // Calcular movimiento relativo al aim direction para strafe
                Vector3 aimRight = Vector3.Cross(aimDir, Vector3.up);
                float rightAmount = Vector3.Dot(moveDir.normalized, aimRight);
                float forwardAmount = Vector3.Dot(moveDir.normalized, aimDir);
                relativeMovement = new Vector3(rightAmount, 0f, forwardAmount);
            }

            // Rotar las piernas
            Quaternion targetRotation = Quaternion.LookRotation(legsDirection);
            visualRoot.rotation = Quaternion.Slerp(
                visualRoot.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            // Actualizar parámetros de animación basados en movimiento relativo a las piernas
            animator.SetFloat(Speed, moveDir.magnitude);
            animator.SetFloat(MoveX, relativeMovement.x);
            animator.SetFloat(MoveY, relativeMovement.z);
        }

        private Vector3 GetAimDirection()
        {
            if (_playerController == null) return transform.forward;

            Vector3 playerPos = transform.position;
            playerPos.y = 0;
            Vector3 mousePos = _playerController.MouseWorldPos;
            mousePos.y = 0;

            Vector3 aimDirection = (mousePos - playerPos).normalized;
            return aimDirection.sqrMagnitude > 0.01f ? aimDirection : transform.forward;
        }

        public void UpdateAimTarget(Vector3 worldPos)
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
                torsoTransform.rotation = Quaternion.Slerp(torsoTransform.rotation, targetRot,
                    Time.deltaTime * torsoRotationSpeed);
            }
        }

        public void UpdateWeaponAim(Vector3 targetPos)
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

        public void SetPlayerController(PlayerController controller)
        {
            _playerController = controller;
        }

        private void SetAnimatorForCurrentMode()
        {
            if (_isInHub && hubAnimator != null)
            {
                animator.runtimeAnimatorController = hubAnimator;
            }
            else if (!_isInHub && combatAnimator != null)
            {
                animator.runtimeAnimatorController = combatAnimator;
            }
        }

        private void UpdateRigBuilderState()
        {
            if (rigBuilder != null)
            {
                rigBuilder.enabled = !_isInHub;
            }
        }

        public void PlayDamageEffect()
        {
            StartCoroutine(FlashCoroutine());
            //La unica forma de comunicarle al view de que el jugador recibio daño es acá
            //En general el view es el que deberia controlar el sonido pero en muchos casos no hay comunicación entre scripts
            //Y como esto no lo codie yo no quiero meterme mucho - Sim.

            audioSource.PlayOneShot(hurtFx);
        }

        private IEnumerator FlashCoroutine()
        {
            // Cambia el color
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    string property = GetColorProperty(mat);

                    if (!string.IsNullOrEmpty(property))
                        mat.SetColor(property, flashColor);
                }
            }

            yield return new WaitForSeconds(flashDuration);

            // Restaura colores
            for (int i = 0; i < renderers.Length; i++)
            {
                var mats = renderers[i].materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    var mat = mats[j];
                    string property = GetColorProperty(mat);

                    if (!string.IsNullOrEmpty(property))
                        mat.SetColor(property, originalColors[i][j]);
                }
            }
        }

        public void PlayHealthEffect()
        {
            audioSource?.PlayOneShot(healthFx);
        }

        private string GetColorProperty(Material mat)
        {
            foreach (var prop in colorPropertyNames)
            {
                if (mat.HasProperty(prop))
                    return prop;
            }

            return null;
        }

        private void UpdateWeaponVisibility()
        {
            if (weaponInstance != null)
            {
                weaponInstance.SetActive(!_isInHub);
            }
        }

        public bool CanUseWeapon()
        {
            return !_isInHub;
        }

        private void OnDrawGizmos()
        {
            if (torsoTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(torsoTransform.position, minAimDistance);
            }

            if (visualRoot != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(visualRoot.position, visualRoot.position + visualRoot.forward * 2f);
            }
        }
    }
}