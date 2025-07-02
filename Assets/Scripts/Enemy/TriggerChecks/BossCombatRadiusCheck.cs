using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCombatRadiusCheck : MonoBehaviour
{

    private BossController _enemyController;
    private BossModel _enemyModel;
    private Transform _playerTransform;

    private void Awake()
    {
        _enemyController = GetComponentInParent<BossController>();
        _enemyModel = GetComponentInParent<BossModel>();


    }

    private void Start()
    {
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

        if (distanceToPlayer > _enemyModel.statsSO.AttackRange)
        {
            _enemyController.SetCombatRadiusBool(false);
            return;
        }

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > _enemyModel.statsSO.combatAngle / 2)
        {
            _enemyController.SetCombatRadiusBool(false);
            return;
        }

        if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, _enemyModel.statsSO.obstacleCombatLayers))
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
        Vector3 rightBoundary = Quaternion.Euler(0, _enemyModel.statsSO.combatAngle / 2, 0) * transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -_enemyModel.statsSO.combatAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * _enemyModel.statsSO.AttackRange);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * _enemyModel.statsSO.AttackRange);
    }
}
