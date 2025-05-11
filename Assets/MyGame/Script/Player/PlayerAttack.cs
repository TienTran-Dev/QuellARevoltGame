using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // Attack
    [SerializeField]
    private float radius;
    [SerializeField]
    private Transform hitPointLeftHand;
    [SerializeField]
    private Transform hitPointRightHand;
    [SerializeField]
    private Transform hitPointLeg;
    [SerializeField]
    private GameObject Effect;
    [SerializeField]
    private LayerMask targetPlayer;
    private Animator animatorAttack;
    private Vector3 attackerPosition;



    //Id animation
    private int _IDAttack_1 = Animator.StringToHash("Attack");
    private int _IDBlock = Animator.StringToHash("Block");
    private bool valueBlock;

    private void Start()
    {
        animatorAttack = GetComponent<Animator>();

    }


    public void OnAttack(InputValue value)
    {

        if (value.isPressed)
        {
            HandleCombo();

        }

    }

    private void HandleCombo()
    {
        animatorAttack.SetTrigger(_IDAttack_1);
    }

    private void BlockInput(bool NewBlockInputValue)
    {
        valueBlock = NewBlockInputValue;
    }

    public void OnBlocked_Attack(InputValue value)
    {

        BlockInput(value.isPressed);
        animatorAttack.SetBool(_IDBlock, valueBlock = true);
        if (!(value.isPressed) || GetComponent<PlayerHealth>().shied < 0)
        {
            animatorAttack.SetBool(_IDBlock, valueBlock = false);
        }

    }

    private void ApplyDamage(Transform attackPoint)
    {
        if (attackPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, radius, targetPlayer);

        foreach (Collider hit in hits)
        {
            var enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(); // truyền damage cụ thể
                Instantiate(Effect, hit.transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }

    public void AttackPointLeftHand() => ApplyDamage(hitPointLeftHand);
    public void AttackPointRightHand() => ApplyDamage(hitPointRightHand);
    public void AttackPointLeg() => ApplyDamage(hitPointLeg);

}


