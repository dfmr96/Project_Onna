using System.Collections;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] private MeleeData data;
    [SerializeField] private Transform attackPoint;

    private MeleeModel model;

    private bool isAttacking = false;
    private bool onCoolDown = false;
    private int comboStep = 0;

    //testing
    private bool showGizmo = false;

    private void Awake() => model = new MeleeModel(data);

    public void Attack()
    {
        if (!isAttacking && !onCoolDown) StartCoroutine(DoAttack());
        //Aca deberia tirar un evento para ejecutar la animacion llamando al view, pero como no se si de eso se va a encargar el player
        //en si por las dudas no lo toco por ahora
    }

    private IEnumerator DoAttack()
    {
        isAttacking = true;
        comboStep++;
        showGizmo = true;

        if (attackPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(attackPoint.position, model.Range);

            foreach (Collider hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && hit.gameObject.layer != 6) damageable.TakeDamage(model.Damage);
            }
        }
        yield return new WaitForSeconds(model.AttackDelay);
        isAttacking = false;
        showGizmo = false;

        if (comboStep >= 2)
        {
            onCoolDown = true;
            yield return new WaitForSeconds(model.CoolDown);
            comboStep = 0;
            onCoolDown = false;
        } 
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, model.Range);
    }
}
