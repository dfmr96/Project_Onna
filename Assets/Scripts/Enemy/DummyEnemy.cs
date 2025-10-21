using UnityEngine;

namespace Enemy
{
    public class DummyEnemy : MonoBehaviour, IDamageable
    {
        public float health = 100f;

        public void TakeDamage(float amount)
        {
            health -= amount;
            Debug.Log($"[DummyEnemy] Took {amount} dmg. Health = {health:F1}");
            if (health <= 0)
            {
                Debug.Log("[DummyEnemy] 💀 Destroyed");
                Destroy(gameObject);
            }
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        public float MaxHealth { get; }
        public float CurrentHealth { get; }
        public Vector3 Transform { get; }
        public void ApplyDebuffDoT(float dotDuration, float dps)
        {
            throw new System.NotImplementedException();
        }
    }
}