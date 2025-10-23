using UnityEngine;
using Player;
using System.Collections;

namespace Enemy
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(EnemyStatusHandler))]
    public class DummyEnemy : MonoBehaviour, IDamageable, ISlowable, IPushable
    {
        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float detectionRadius = 3.5f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float patrolDistance = 5f;
        [SerializeField] private bool canMove = true;

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
        private Coroutine pushbackRoutine;

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
            if (!canMove) return;

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

            float finalDamage = attackDamage;

            // Apply outgoing damage multiplier from DamageReductionEffect
            var statusHandler = GetComponent<EnemyStatusHandler>();
            if (statusHandler != null)
            {
                float multiplier = statusHandler.GetOutgoingDamageMultiplier();
                if (multiplier < 1f)
                {
                    Debug.Log($"[DummyEnemy] 😵 WEAKENED! Damage {attackDamage:F1} → {attackDamage * multiplier:F1} ({multiplier:P0} multiplier)");
                }
                finalDamage *= multiplier;
            }

            Debug.Log($"[DummyEnemy] 💢 Attacking player for {finalDamage:F1} dmg (interval={currentAttackInterval:F2}s)");
            targetPlayer.TakeDamage(finalDamage);
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
            // Apply damage multiplier from WeakenEffect
            var statusHandler = GetComponent<EnemyStatusHandler>();
            if (statusHandler != null)
            {
                float multiplier = statusHandler.GetDamageMultiplier();
                if (multiplier > 1f)
                {
                    Debug.Log($"[DummyEnemy] 💥 WEAKENED! Damage {amount:F1} → {amount * multiplier:F1} ({multiplier:P0} multiplier)");
                }
                amount *= multiplier;
            }

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

        // -----------------------------
        // IPushable Implementation
        // -----------------------------
        public void ApplyPushback(Vector3 direction, float force)
        {
            if (pushbackRoutine != null)
                StopCoroutine(pushbackRoutine);

            pushbackRoutine = StartCoroutine(PushbackRoutine(direction, force));
        }

        private IEnumerator PushbackRoutine(Vector3 direction, float force)
        {
            // Visual feedback
            if (rend)
                rend.material.color = Color.yellow;

            // Aplicar pushback solo en el plano XZ (sin cambio en Y)
            direction.y = 0f;
            direction.Normalize();

            float pushDistance = force * 0.1f; // Convertir fuerza a distancia
            Vector3 currentPos = transform.position;
            Vector3 targetPos = currentPos + direction * pushDistance;

            // Mantener la Y original
            targetPos.y = currentPos.y;

            float elapsed = 0f;
            float duration = 0.2f;
            Vector3 startPos = transform.position;

            Debug.Log($"[DummyEnemy] 💨 Pushed! Direction={direction}, Force={force}, Distance={pushDistance:F2}");

            // Mover suavemente hacia la posición objetivo (solo en XZ)
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
                newPos.y = startPos.y; // Forzar Y constante durante el lerp
                transform.position = newPos;
                yield return null;
            }

            // Asegurar posición final con Y original
            targetPos.y = startPos.y;
            transform.position = targetPos;

            // Restaurar color
            if (rend)
                rend.material.color = baseColor;

            pushbackRoutine = null;
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
