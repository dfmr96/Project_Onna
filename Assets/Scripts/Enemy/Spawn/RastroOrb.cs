using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Enemy.Spawn
{
    public class RastroOrb : MonoBehaviour
    {
        public static event Action<float> OnOrbCollected;

        [Header("Floating Settings")]
        [SerializeField] private float minFloatSpeed = 0.2f;
        [SerializeField] private float maxFloatSpeed = 1f;
        [SerializeField] private float minFloatAmplitude = 0.2f;
        [SerializeField] private float maxFloatAmplitude = 1f;
        private float floatSpeed;
        private float floatAmplitude;

        [Header("Healing Settings")]
        [SerializeField] private float healingAmount = 10f;
    
        [Header("Lifetime Settings")]
        [SerializeField] private float lifetime = 5f;

        [Header("Attraction Settings")]
        [SerializeField] private float baseAttractionRadius = 5f;
        [SerializeField] private float baseAttractionSpeed = 5f;
        
        private Transform attractionTarget;
        private Action _onCollected;
        private float timer;
        private Vector3 startPos;
        private GameObject playerGO;
        private PlayerModel playerModel;

        // Propiedades calculadas que incluyen stats del player
        private float attractionRadius => GetCurrentAttractionRadius();
        private float attractionSpeed => GetCurrentAttractionSpeed();
    
        //----------------------------------------------------------------------
        // Unity Methods
        //----------------------------------------------------------------------
        private void OnEnable()
        {
            timer = lifetime;
            attractionTarget = null;
        }

        void Start()
        {
            startPos = transform.position;
            GetPlayer();

            floatSpeed = UnityEngine.Random.Range(minFloatSpeed, maxFloatSpeed);
            floatAmplitude = UnityEngine.Random.Range(minFloatAmplitude, maxFloatAmplitude);
        }
        
        void Update()
        {
            timer -= Time.deltaTime;
            HandleFloating();
            HandleLifetime();
            HandleAttraction();
        }
    
        private void OnTriggerEnter(Collider other)
        { 
            if (!IsPlayer(other)) return;
            HealPlayer();
            OnOrbCollected?.Invoke(healingAmount);
            _onCollected?.Invoke();


            var effectController = other.GetComponent<PlayerControllerEffect>();
            if (effectController != null)
            {
                //aplica efecto Alpha Major si está activa
                effectController.ApplyAlphaMajorEffect();
                effectController.ApplyAlphaMinorEffect();

            }


            DeactivateOrb();
        }
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attractionRadius);
        }
    
        //----------------------------------------------------------------------
        // Public methods
        //----------------------------------------------------------------------
        public void Init(Action onCollected)
        {
            _onCollected = onCollected;
        }
    
        //----------------------------------------------------------------------
        // Private methods
        //----------------------------------------------------------------------
    
        private void GetPlayer()
        {
            playerGO = PlayerHelper.GetPlayer();

            if (playerGO == null)
            {
                Debug.LogWarning("[ORB] No player found. Attraction will not work.");
                return;
            }

            playerModel = playerGO.GetComponent<PlayerModel>();
            if (playerModel == null)
            {
                Debug.LogWarning("[ORB] PlayerModel not found. Using base attraction values.");
            }
        }
        
        private void HandleFloating()
        {
            Vector3 position = transform.position;
            position.y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = position;
        }
        private void HandleLifetime()
        {
            if (timer <= 0f)
            {
                //Si en el tiempo no lo toma el player se desactiva
                DeactivateOrb();
            }
        }
    
        private void HandleAttraction()
        {
            if (attractionTarget == null)
            {
                TryStartAttraction();
            }
            else
            {
                MoveTowardsAttractionTarget();
            }
        }
    
        private void TryStartAttraction()
        {
            if (playerGO == null)
                return;

            float distance = Vector3.Distance(transform.position, playerGO.transform.position);
            if (distance <= attractionRadius)
            {
                attractionTarget = playerGO.transform;
            }
        }
    
        private void MoveTowardsAttractionTarget()
        {
            float distance = Vector3.Distance(transform.position, attractionTarget.position);
            if (distance > attractionRadius)
            {
                attractionTarget = null;
                return;
            }

            Vector3 direction = (attractionTarget.position - transform.position).normalized;
            transform.Translate(direction * (attractionSpeed * Time.deltaTime), Space.Self);
        }
        private bool IsPlayer(Collider other)
        {
            return other.gameObject == playerGO;
        }
        private void HealPlayer()
        {
            if (playerGO.TryGetComponent(out IHealable healable))
            {
                healable.RecoverTime(healingAmount);
            }
        }

        private void DeactivateOrb()
        {
            attractionTarget = null;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Obtiene el radio de atracción actual considerando los stats del player
        /// </summary>
        private float GetCurrentAttractionRadius()
        {
            if (playerModel?.StatContext?.Source != null && playerModel.StatRefs.orbAttractRange != null)
            {
                float playerRange = playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractRange);
                return baseAttractionRadius + playerRange; // Base + bonus del player
            }

            return baseAttractionRadius; // Fallback a valor base
        }

        /// <summary>
        /// Obtiene la velocidad de atracción actual considerando los stats del player
        /// </summary>
        private float GetCurrentAttractionSpeed()
        {
            if (playerModel?.StatContext?.Source != null && playerModel.StatRefs.orbAttractSpeed != null)
            {
                float playerSpeedMultiplier = 1f + playerModel.StatContext.Source.Get(playerModel.StatRefs.orbAttractSpeed);
                return baseAttractionSpeed * playerSpeedMultiplier; // Base * multiplicador del player
            }

            return baseAttractionSpeed; // Fallback a valor base
        }
    }
}
