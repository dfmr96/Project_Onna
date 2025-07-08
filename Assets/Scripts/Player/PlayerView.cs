using System;
using System.Collections;
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
        [SerializeField] private AudioClip hurtFx;
        
        [Header("Params")]
        [SerializeField] private float torsoRotationSpeed = 10f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float minAimDistance = 2f;
        
        private PlayerController _playerController;
        private PlayerInputHandler _playerInputHandler;
        private static readonly int MoveX = Animator.StringToHash("MoveX");
        private static readonly int MoveY = Animator.StringToHash("MoveY");
        private static readonly int Speed = Animator.StringToHash("Speed");

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
                torsoTransform.rotation = Quaternion.Slerp(torsoTransform.rotation, targetRot, Time.deltaTime * torsoRotationSpeed);
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

        private string GetColorProperty(Material mat)
        {
            foreach (var prop in colorPropertyNames)
            {
                if (mat.HasProperty(prop))
                    return prop;
            }
            return null;
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