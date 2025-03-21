using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private float m_AttackTime;
    private int m_AttackClick;
    // Attack
    [SerializeField]
    private float radius;
    [SerializeField]
    private Transform[] hitPoint;
    [SerializeField]
    private int dame;
    [SerializeField]
    private GameObject Effect;
    [SerializeField]
    private LayerMask targetPlayer;
    private Animator animatorAttack;

   
    //Id animation
    private int _IDAttack_1 = Animator.StringToHash("Attack");

    private void Start()
    {
        animatorAttack =  GetComponent<Animator>();

    }
    private void Update()
    {
        AttackPoint();
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


    public void AttackPoint()
    {
        if (hitPoint != null)
        {
            foreach (Transform t in hitPoint)
            {
                Collider[] hit = Physics.OverlapSphere(t.position, radius, targetPlayer);// tạo hình cầu ảo để gây dame.

                if (hit.Length > 0)// đảm bảo có 1 đối tượng tác động.
                {
                    hit[0].GetComponent<PlayerHealth>().TakeDamage(dame);// tạo list collider xem các gameobject có component health nhận dame.
                    Instantiate(Effect.transform, hit[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); // tạo bản sao effect.
                }
            }
        }
    }

}
