using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class EnemyAggroCheck : MonoBehaviour
{
    private EnemyController _enemyController;
    private Transform _playerTransform;
    private EnemyModel _enemyModel;

    private void Awake()
    {
        _enemyController = GetComponentInParent<EnemyController>();
        _enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void Start()
    {
        _playerTransform = PlayerHelper.GetPlayer().transform;
    }

    private void Update()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        if (_playerTransform == null) return;

        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer > _enemyModel.statsSO.detectionRange)
        {
            _enemyController.SetAggroStatus(false);
            return;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > _enemyModel.statsSO.visionAngle / 2)
        {
            _enemyController.SetAggroStatus(false);
            return;
        }

        // Lógica SphereCast como CanChargeToPlayer
        Vector3 origin = transform.position + Vector3.up * 1f;
        float rayRadius = 0.5f;

        bool canSeePlayer = true;
        if (Physics.SphereCast(origin, rayRadius, directionToPlayer, out RaycastHit hit, distanceToPlayer))
        {
            if (hit.transform != _playerTransform)
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                canSeePlayer = false;
            }
        }

        Debug.DrawLine(origin, _playerTransform.position, canSeePlayer ? Color.green : Color.red);
        _enemyController.SetAggroStatus(canSeePlayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerTransform == null || _enemyModel == null) return;

        Gizmos.color = Color.red;
        Vector3 rightBoundary = Quaternion.Euler(0, _enemyModel.statsSO.visionAngle / 2, 0) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -_enemyModel.statsSO.visionAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * _enemyModel.statsSO.detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * _enemyModel.statsSO.detectionRange);
    }
}
