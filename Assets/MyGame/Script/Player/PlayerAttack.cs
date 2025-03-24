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
        if (!(value.isPressed))
        {
            animatorAttack.SetBool(_IDBlock, valueBlock = false);
        }

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
                        hit[0].GetComponent<PlayerHealth>().TakeDamage(dame);
                    
                    // tạo list collider xem các gameobject có component health nhận dame.
                    Instantiate(Effect.transform, hit[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); // tạo bản sao effect.
                }

            }

        }

        //if (hitPoint == null ) return; // Tránh lỗi nếu không có hitPoint nào

        //foreach (Transform t in hitPoint)
        //{
        //    Collider[] hitTargets = Physics.OverlapSphere(t.position, radius, targetPlayer);
        //    //Debug.Log($"{hitPoint}");

        //    foreach (var target in hitTargets)
        //    {


        //        PlayerHealth health = target.GetComponent<PlayerHealth>();


        //        if (health != null) // Kiểm tra có PlayerHealth không
        //        {
        //            health.TakeDamage(dame);
        //            Instantiate(Effect, target.transform.position + Vector3.up, Quaternion.identity);
        //        }

        //    }
        //}
    }

}


