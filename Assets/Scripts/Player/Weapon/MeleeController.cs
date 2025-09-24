using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] private MeleeData data;
    [SerializeField] private Transform attackPoint;

    public MeleeModel Model { get; private set; }

    private bool isAttacking = false;
    private bool onCoolDown = false;
    private int comboStep = 0;
    private float lastAttackTime = 0f;

    //testing
    private bool showGizmo = false;

    private void Awake() => Model = new MeleeModel(data);

    public void Attack()
    {
        if (Time.time - lastAttackTime > Model.TimeBetweenCombo) comboStep = 0;
        if (!isAttacking && !onCoolDown) StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        comboStep++;
        showGizmo = true;

        Model.StartAttack(comboStep);

        if (attackPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(attackPoint.position, Model.Range);

            foreach (Collider hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && hit.gameObject.layer != 6) damageable.TakeDamage(Model.Damage);
            }
        }
        yield return new WaitForSeconds(Model.AttackDelay);
        isAttacking = false;
        showGizmo = false;

        if (comboStep >= 2)
        {
            onCoolDown = true;
            yield return new WaitForSeconds(Model.CoolDown);
            comboStep = 0;
            onCoolDown = false;
        } 
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, Model.Range);
    }
}
