using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed;
    private float _damage;
    private float _maxDistance;

    [SerializeField] private GameObject hitParticlePrefab;

    [SerializeField] private LayerMask ignoreLayers;

    private void Start()
    {
        float destroyTime = _maxDistance / _bulletSpeed;
        Destroy(gameObject, destroyTime);
    }

    private void Update() { Move(); }

    private void Move() { transform.Translate(Vector3.forward * (_bulletSpeed * Time.deltaTime)); }

    public void Setup(float speed, float distance, float damage)
    {
        _bulletSpeed = speed;
        _maxDistance = distance;
        this._damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((ignoreLayers.value & (1 << other.gameObject.layer)) != 0)
            return;

        // instanciar partículas siempre al colisionar
        if (hitParticlePrefab != null)
        {
            GameObject hitVFX = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            Destroy(hitVFX, 2f); // se destruye a los 2 segundos
        }


        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_damage);

        Destroy(gameObject);
    }

}