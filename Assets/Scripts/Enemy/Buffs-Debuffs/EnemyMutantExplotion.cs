using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Unity.VisualScripting.Antlr3.Runtime.Collections;

public class EnemyMutantExplotion : MonoBehaviour
{
    protected Transform playerTransform;

    [SerializeField] private EnemyVariantSO variantSO;

    private float damageMultiplier;
    private float radiusMultiplier;

    private bool hasHit = false;




    private void Start()
    {
        playerTransform = PlayerHelper.GetPlayer().transform;

        damageMultiplier = variantSO.explosionDamage;
        radiusMultiplier = variantSO.explosionRadius;
        transform.localScale = Vector3.one * radiusMultiplier;



    }


    private void OnTriggerEnter(Collider collision)
    {

        if (hasHit) return;

        if ((collision.transform.root == playerTransform) && (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable)))
        {
            hasHit = true;

            damageable.TakeDamage(damageMultiplier);


        }

    }

}
