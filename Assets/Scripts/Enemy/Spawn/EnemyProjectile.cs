using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System;

public class EnemyProjectile : MonoBehaviour
{
    private float damage;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private ParticleSystem impactEffectParticlesPrefab;
    protected Transform playerTransform;
    [SerializeField] float bulletSpeed;
    [SerializeField] private LayerMask obstacleLayers;
    private bool hasHit = false;
    private Action onRelease;
    private Rigidbody rb;
    private float _timer = 0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Start()
    {
        playerTransform = PlayerHelper.GetPlayer().transform;

    }

    public void Launch(Vector3 direction, float force, float damage, Action onReleaseCallback)
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

        if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
        {
            PlayImpactParticles();
            onRelease?.Invoke();

        }


        if ((collision.transform.root == playerTransform) && (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)))
        {
            hasHit = true;

            damageable.TakeDamage(damage);
            Debug.Log("Player damaged");

            PlayImpactParticles();


            onRelease?.Invoke();


        }

    }
 
}

