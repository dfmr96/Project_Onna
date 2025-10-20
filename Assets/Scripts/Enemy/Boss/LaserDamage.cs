using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;


public class LaserDamage : MonoBehaviour
{
    private float damagePerSecond = 10f;
    private float tickRate = 0.2f;
    private float laserLength = 20f;
    // velocidad angular en grados/seg
    private float rotationSpeed = 60f;

    private bool isActive = false;
    private Coroutine damageCoroutine;
    private LineRenderer lineRenderer;

    public Transform laserOrigin;
    private Transform playerTransform;
    private IDamageable playerDamageable;

    [SerializeField] private ParticleSystem impactEffectParticlesPrefab;
    private ParticleSystem impactParticlesInstance;

    private BossModel _bossModel;
    private BossView _bossView;

    // nueva: dirección actual del rayo
    private Vector3 currentDir;



    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        _bossModel = GetComponentInParent<BossModel>();
        _bossView = GetComponentInParent<BossView>();

        playerTransform = PlayerHelper.GetPlayer().transform;
        playerDamageable = playerTransform.GetComponent<IDamageable>();

        if (impactEffectParticlesPrefab != null)
        {
            impactParticlesInstance = Instantiate(impactEffectParticlesPrefab);
            impactParticlesInstance.Stop();
        }

        // inicializo desde stats
        damagePerSecond = _bossModel.statsSO.LaserDamagePerSecond;
        laserLength = _bossModel.statsSO.LaserLenght;
        tickRate = _bossModel.statsSO.LaserTickRate;
        rotationSpeed = _bossModel.statsSO.LaserRotationSpeed;

        currentDir = laserOrigin.forward; // dirección inicial
    }

    private void Update()
    {
        if (!isActive) return;

        // posición del player con offset en Y
        Vector3 targetPos = playerTransform.position;
        targetPos.y += 1.0f;

        // dirección hacia el player
        Vector3 targetDir = (targetPos - laserOrigin.position).normalized;

        // rotación lenta
        currentDir = Vector3.RotateTowards(
            currentDir,
            targetDir,
            rotationSpeed * Mathf.Deg2Rad * Time.deltaTime,
            1f
        );

        // calcular fin del rayo
        Vector3 endPos = laserOrigin.position + currentDir * laserLength;
        if (Physics.Raycast(new Ray(laserOrigin.position, currentDir), out RaycastHit hit, laserLength))
        {
            endPos = hit.point;
        }

        // dibujar línea
        lineRenderer.SetPosition(0, laserOrigin.position);
        lineRenderer.SetPosition(1, endPos);

        // partículas
        if (impactParticlesInstance != null)
        {
            impactParticlesInstance.transform.position = endPos;
            Vector3 toLaserOrigin = (laserOrigin.position - endPos).normalized;
            if (toLaserOrigin != Vector3.zero)
                impactParticlesInstance.transform.rotation = Quaternion.LookRotation(toLaserOrigin);

            if (!impactParticlesInstance.isPlaying)
                impactParticlesInstance.Play();
        }
    }

    public void StartLaser()
    {
        isActive = true;
        lineRenderer.enabled = true;

        if (damageCoroutine == null)
            damageCoroutine = StartCoroutine(DamageRoutine());

        if (impactParticlesInstance != null && !impactParticlesInstance.isPlaying)
            impactParticlesInstance.Play();

        _bossView.StartLaserShoot();
    }

    public void StopLaser()
    {
        isActive = false;
        lineRenderer.enabled = false;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        if (impactEffectParticlesPrefab != null)
            impactParticlesInstance.Stop();

        _bossView.StopLaserShoot();
    }

    private IEnumerator DamageRoutine()
    {
        while (isActive)
        {
            if (Physics.Raycast(new Ray(laserOrigin.position, currentDir), out RaycastHit hit, laserLength))
            {
                if (hit.transform == playerTransform)
                {
                    float damage = damagePerSecond * tickRate;
                    Debug.Log("Applying damage: " + damage);
                    playerDamageable?.TakeDamage(damage);
                }
            }
            yield return new WaitForSeconds(tickRate);
        }
    }
}
