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

    public void IsBlocked()
    {
        
    }

    public void AttackPointLeftHand()
    {
        if (hitPointLeftHand != null)
        {
          
            
                Collider[] hit = Physics.OverlapSphere(hitPointLeftHand.position,radius, targetPlayer);// tạo hình cầu ảo để gây dame.

                if (hit.Length > 0)// đảm bảo có 1 đối tượng tác động.
                {
                    hit[0].GetComponent<EnemyHealth>().TakeDamage();

                    // tạo list collider xem các gameobject có component health nhận dame.
                    Instantiate(Effect.transform, hit[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); // tạo bản sao effect.

                }
            
                
        }

       
    }
    public void AttackPointRightHand()
    {
        if (hitPointRightHand != null)
        {
            
            
                Collider[] hit = Physics.OverlapSphere(hitPointRightHand.position, radius, targetPlayer);// tạo hình cầu ảo để gây dame.

                if (hit.Length > 0)// đảm bảo có 1 đối tượng tác động.
                {
                    hit[0].GetComponent<EnemyHealth>().TakeDamage();

                    // tạo list collider xem các gameobject có component health nhận dame.
                    Instantiate(Effect.transform, hit[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); // tạo bản sao effect.

                }
            
            
        
        }


    }
    public void AttackPointLeg()
    {
        if (hitPointLeg != null)
        {

            
                Collider[] hit = Physics.OverlapSphere(hitPointLeg.position, radius, targetPlayer);// tạo hình cầu ảo để gây dame.

                if (hit.Length > 0)// đảm bảo có 1 đối tượng tác động.
                {
                    hit[0].GetComponent<EnemyHealth>().TakeDamage();

                    // tạo list collider xem các gameobject có component health nhận dame.
                    Instantiate(Effect.transform, hit[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); // tạo bản sao effect.

                }
            
                
        }


    }
}


