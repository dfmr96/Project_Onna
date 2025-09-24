using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed;
    private float _damage;
    private float _maxDistance;

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
        //if (other.TryGetComponent<IDamageable>(out var damageable))
        //    damageable.TakeDamage(_damage);
        //Destroy(gameObject);

        // si la capa está en las capas a ignorar => no hacer nada
        if ((ignoreLayers.value & (1 << other.gameObject.layer)) != 0)
            return;

        // aplicar daño si corresponde
        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_damage);

        // destruir siempre (menos en las capas ignoradas)
        Destroy(gameObject);
    }

}