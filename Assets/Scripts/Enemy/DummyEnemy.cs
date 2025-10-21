using UnityEngine;
using Player;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class DummyEnemy : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float detectionRadius = 3.5f;

        [Header("Debug")]
        [SerializeField] private Color detectionColor = new Color(1f, 0.3f, 0.3f, 0.3f);

        private float currentHealth;
        private float attackTimer;
        private PlayerModel targetPlayer;

        private void Awake()
        {
            currentHealth = maxHealth;

            // Ajustamos el collider para representar el área de ataque
            var sphere = GetComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = detectionRadius;
        }

        private void Update()
        {
            if (targetPlayer == null) return;

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                Attack();
            }
        }

        private void Attack()
        {
            if (targetPlayer == null) return;

            Debug.Log($"[DummyEnemy] 💢 Attacking player for {attackDamage} dmg");
            targetPlayer.TakeDamage(attackDamage);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (targetPlayer != null) return;

            if (other.TryGetComponent<PlayerModel>(out var player))
            {
                targetPlayer = player;
                Debug.Log("[DummyEnemy] 🎯 Player detected, preparing to attack...");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerModel>(out var player) && player == targetPlayer)
            {
                targetPlayer = null;
                Debug.Log("[DummyEnemy] Player left range.");
            }
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"[DummyEnemy] Took {amount:F1} dmg. Health = {currentHealth:F1}");

            if (currentHealth <= 0)
            {
                Debug.Log("[DummyEnemy] 💀 Destroyed");
                Destroy(gameObject);
            }
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;

        // No usado en dummy, pero requerido por interfaz
        public Vector3 Transform => transform.position;

        public void ApplyDebuffDoT(float dotDuration, float dps)
        {
            Debug.Log($"[DummyEnemy] Received DoT {dps} DPS for {dotDuration}s");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
