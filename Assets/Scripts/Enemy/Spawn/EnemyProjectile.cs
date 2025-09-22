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
    protected Transform playerTransform;
    [SerializeField] float bulletSpeed;
    [SerializeField] private LayerMask ignoreLayers;
    private bool hasHit = false;
    private Action onRelease;
    private Rigidbody rb;

    private EnemyModel ownerModel;



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

    private void PlayImpactParticles()
    {
        if (impactEffectParticlesPrefab != null)
        {
            var impact = Instantiate(impactEffectParticlesPrefab, transform.position, Quaternion.identity);
            impact.Play();
            Destroy(impact.gameObject, impact.main.duration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (hasHit) return;


        //if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
        //{
        //    PlayImpactParticles();
        //    onRelease?.Invoke();

        //}

        // Ignoramos colisiones con capas de ignoreLayers
        if (((1 << collision.gameObject.layer) & ignoreLayers) != 0) return;


        if ((collision.transform.root == playerTransform) && (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)))
        {
            hasHit = true;

            damageable.TakeDamage(damage);

            //Debuff veneno
            if (ownerModel != null && ownerModel.variantSO.variantType == EnemyVariantType.Green)
            {
                damageable.ApplyDebuffDoT(ownerModel.variantSO.dotDuration, ownerModel.variantSO.dotDamage);
            }

            PlayImpactParticles();


            onRelease?.Invoke();


        }
        else
        {
            hasHit = true;

            PlayImpactParticles();

            onRelease?.Invoke();

        }

    }
 
}

