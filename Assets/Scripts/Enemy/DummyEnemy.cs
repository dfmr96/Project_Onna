using UnityEngine;
using Player;
using System.Collections;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    public class DummyEnemy : MonoBehaviour, IDamageable, ISlowable
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float detectionRadius = 3.5f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float patrolDistance = 5f;

        [Header("Debug")]
        [SerializeField] private Color detectionColor = new Color(1f, 0.3f, 0.3f, 0.3f);
        [SerializeField] private bool showDebug = true;

        private float currentHealth;
        private float attackTimer;
        private float currentAttackInterval;
        private float currentMoveSpeed;
        private Vector3 startPos;
        private int direction = 1;

        private PlayerModel targetPlayer;
        private Coroutine slowRoutine;
        private Renderer rend;
        private Color baseColor;

        private void Awake()
        {
            currentHealth = maxHealth;
            currentAttackInterval = attackInterval;
            currentMoveSpeed = moveSpeed;
            startPos = transform.position;

            // Collider de detección / ataque
            var sphere = GetComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = detectionRadius;

            rend = GetComponentInChildren<Renderer>();
            if (rend)
                baseColor = rend.material.color;
        }

        private void Update()
        {
            HandlePatrol();
            HandleAttack();
        }

        private void HandlePatrol()
        {
            // Movimiento de lado a lado entre -patrolDistance y +patrolDistance
            Vector3 pos = transform.position;
            pos += transform.right * currentMoveSpeed * direction * Time.deltaTime;
            transform.position = pos;

            if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
            {
                direction *= -1;
                transform.localScale = new Vector3(direction, 1, 1); // girar visualmente
            }
        }

        private void HandleAttack()
        {
            if (targetPlayer == null) return;

            attackTimer += Time.deltaTime;
            if (attackTimer >= currentAttackInterval)
            {
                attackTimer = 0f;
                Attack();
            }
        }

        private void Attack()
        {
            if (targetPlayer == null) return;

            Debug.Log($"[DummyEnemy] 💢 Attacking player for {attackDamage} dmg (interval={currentAttackInterval:F2}s)");
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

        // -----------------------------
        // IDamageable Implementation
        // -----------------------------
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

        public void Die() => Destroy(gameObject);
        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public Vector3 Transform => transform.position;

        public void ApplyDebuffDoT(float dotDuration, float dps)
        {
            Debug.Log($"[DummyEnemy] Received DoT {dps} DPS for {dotDuration}s");
        }

        // -----------------------------
        // ISlowable Implementation
        // -----------------------------
        public void ApplySlow(float multiplier, float duration)
        {
            if (slowRoutine != null)
                StopCoroutine(slowRoutine);

            slowRoutine = StartCoroutine(SlowRoutine(multiplier, duration));
        }

        private IEnumerator SlowRoutine(float mult, float dur)
        {
            // Cambiar color visual
            if (rend)
                rend.material.color = Color.cyan;

            // Aplicar slow (reduce velocidad y frecuencia de ataque)
            currentMoveSpeed = moveSpeed * mult;
            currentAttackInterval = attackInterval / mult; // ataca más lento (intervalo mayor)

            Debug.Log($"[DummyEnemy] 🧊 Slowed ({mult:P0}) → moveSpeed={currentMoveSpeed:F2}, atkInterval={currentAttackInterval:F2}");

            yield return new WaitForSeconds(dur);

            // Restaurar valores originales
            currentMoveSpeed = moveSpeed;
            currentAttackInterval = attackInterval;

            if (rend)
                rend.material.color = baseColor;

            slowRoutine = null;
            Debug.Log("[DummyEnemy] 🟢 Slow expired, back to normal speed.");
        }

        private void OnDrawGizmosSelected()
        {
            if (!showDebug) return;
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPos + Vector3.left * patrolDistance, startPos + Vector3.right * patrolDistance);
        }
    }
}
