using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class LaserDamage : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float tickRate = 0.2f;
    [SerializeField] private float laserLength = 20f;

    private bool isActive = false;
    private Coroutine damageCoroutine;
    private LineRenderer lineRenderer;

    public Transform laserOrigin;
    private Transform playerTransform;
    private IDamageable playerDamageable;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        playerTransform = PlayerHelper.GetPlayer().transform;
        playerDamageable = playerTransform.GetComponent<IDamageable>();
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 direction = laserOrigin.forward;
        Vector3 endPos = laserOrigin.position + direction * laserLength;

        Ray ray = new Ray(laserOrigin.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserLength))
        {
            endPos = hit.point;
        }

        lineRenderer.SetPosition(0, laserOrigin.position);
        lineRenderer.SetPosition(1, endPos);
    }

    public void StartLaser()
    {
        isActive = true;
        lineRenderer.enabled = true;

        if (damageCoroutine == null)
            damageCoroutine = StartCoroutine(DamageRoutine());
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
    }

    private IEnumerator DamageRoutine()
    {
        while (isActive)
        {
            Ray ray = new Ray(laserOrigin.position, laserOrigin.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, laserLength))
            {
                if (hit.transform == playerTransform)
                {
                    playerDamageable?.TakeDamage(damagePerSecond * tickRate);
                }
            }

            yield return new WaitForSeconds(tickRate);
        }
    }

}

