using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System;

public class EnemyProjectile : MonoBehaviour
{
    private float damage;
    [SerializeField] private float lifeTime = 5f;
    private float _timer = 0f;

    [SerializeField] private ParticleSystem impactEffectParticlesPrefab;
    [SerializeField] private TrailRenderer trail;

    protected Transform playerTransform;
    [SerializeField] float bulletSpeed;
    [SerializeField] private LayerMask ignoreLayers;
    private bool hasHit = false;
    private Action onRelease;
    private Rigidbody rb;

    private EnemyModel ownerModel;
    private BossModel ownerBModel;




    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        playerTransform = PlayerHelper.GetPlayer().transform;

    }

    public void Launch(Vector3 direction, float force, float damage, EnemyModel owner, Action onReleaseCallback)
    {
        this.damage = damage;
        onRelease = onReleaseCallback;
        rb.velocity = direction * force;
        this.ownerModel = owner;
        _timer = 0f;
        hasHit = false;

    
    }


    public void LaunchBoss(Vector3 direction, float force, float damage, Action onReleaseCallback)
    {
        this.damage = damage;
        onRelease = onReleaseCallback;
        rb.velocity = direction * force;
        _timer = 0f;
        hasHit = false;

        // Reactivar trail después de disparar
        if (trail != null)
            trail.emitting = true;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= lifeTime)
        {
            onRelease?.Invoke();
            _timer = 0f;
            hasHit = false;

        }
    }

    private void PlayImpactParticles(Vector3 position, Vector3 normal)
    {
        if (impactEffectParticlesPrefab != null)
        {
            var impact = Instantiate(impactEffectParticlesPrefab, position, Quaternion.LookRotation(normal));
            impact.Play();
            Destroy(impact.gameObject, impact.main.duration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        // ignorar capas
        if (((1 << collision.gameObject.layer) & ignoreLayers) != 0) return;

        hasHit = true;

        // punto de contacto principal
        ContactPoint contact = collision.contacts[0];

        if ((collision.transform.root == playerTransform) && 
            collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);

            // aplicar veneno si corresponde
            if (ownerModel != null && ownerModel.variantSO.variantType == EnemyVariantType.Green)
            {
                damageable.ApplyDebuffDoT(ownerModel.variantSO.dotDuration, ownerModel.variantSO.dotDamage);
            }
        }

        // siempre instanciamos partículas en el punto de impacto
        PlayImpactParticles(contact.point, contact.normal);

        onRelease?.Invoke();
    }


    public void ResetIdle(System.Action releaseAction)
    {
        onRelease = releaseAction;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;  // congelamos mientras está en la mano

        ResetTrail();

    }

    public void Fire(Vector3 direction, float force, float damage, EnemyModel owner)
    {

        this.damage = damage;
        this.ownerModel = owner;
        _timer = 0f;
        hasHit = false;

        rb.isKinematic = false;
        rb.velocity = direction * force;

        // Reactivar trail después de disparar
        if (trail != null)
            trail.emitting = true;
    }

    public void FireBoss(Vector3 direction, float force, float damage, BossModel owner)
    {

        this.damage = damage;
        this.ownerBModel = owner;
        _timer = 0f;
        hasHit = false;

        rb.isKinematic = false;
        rb.velocity = direction * force;

        // Reactivar trail después de disparar
        if (trail != null)
            trail.emitting = true;
    }

    public void ResetTrail()
    {
        if (trail != null)
        {
            trail.Clear();       // limpia el trail viejo
            trail.emitting = false; // desactiva temporalmente
        }

    }
}

