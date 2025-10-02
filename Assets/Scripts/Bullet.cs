using Mutations;
using Mutations.Core;
using System;
using UnityEngine;
using Player;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed;
    private float _damage;
    private float _maxDistance;

    private GameObject playerGO;

    [SerializeField] private GameObject hitParticlePrefab;

    [SerializeField] private LayerMask ignoreLayers;

    private List<BulletModifierSO> _modifiers = new List<BulletModifierSO>();
    private PlayerControllerEffect _playerEffect;

    private void Start()
    {
        float destroyTime = _maxDistance / _bulletSpeed;
        Destroy(gameObject, destroyTime);
        playerGO = PlayerHelper.GetPlayer();

        //// Llamar OnSetup de cada modificador
        //foreach (var mod in _modifiers)
        //    mod.OnSetup(this, playerGO.GetComponent<PlayerControllerEffect>());
    }

    public void RegisterModifier(BulletModifierSO modifier, PlayerControllerEffect player)
    {
        if (!_modifiers.Contains(modifier))
        {
            _modifiers.Add(modifier);
            Debug.Log("[Bullet] Modifier registrado: " + modifier.name);

        }
        _playerEffect = player;
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
            Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
        }

        if (other.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(_damage);

        //MUTACION
        //   var collectable = other.GetComponent<IProjectileCollectable>();
        //if (collectable != null)
        //{
        //    var playerEffect = playerGO.GetComponent<PlayerControllerEffect>();
        //    collectable.OnHitByProjectile(playerEffect);
        //}

        // Aplicar todos los modificadores
        foreach (var mod in _modifiers)
            mod.OnHit(this, other.gameObject, _playerEffect);

        Destroy(gameObject);
    }

}