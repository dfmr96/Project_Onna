using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatRadiusCheck : MonoBehaviour
{
    [Header("Vision Combat Stats")]
    [SerializeField] private float combatRange = 5f; 
    [SerializeField] private float combatAngle = 30f; 
    [SerializeField] private LayerMask obstacleLayers; 

    private EnemyController _enemyController;
    private Transform _playerTransform;

    private void Awake()
    {
        _enemyController = GetComponentInParent<EnemyController>();
        //_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _playerTransform = PlayerHelper.GetPlayer().transform;
            
    }

    private void Update()
    {
        CheckForCombat();
    }

    private void CheckForCombat()
    {
        if (_playerTransform == null) return;

        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer > combatRange)
        {
            _enemyController.SetCombatRadiusBool(false);
            return;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > combatAngle / 2)
        {
            _enemyController.SetCombatRadiusBool(false);
            return;
        }

        if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayers))
        {
            _enemyController.SetCombatRadiusBool(true);
        }
        else
        {
            _enemyController.SetCombatRadiusBool(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerTransform == null) return;

        Gizmos.color = Color.blue;
        Vector3 rightBoundary = Quaternion.Euler(0, combatAngle / 2, 0) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -combatAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * combatRange);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * combatRange);
    }
}
