using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static InputSystem _input;
    public static PlayerHealth InstancePlayerHealth;
    public int health;
    public int shied;
    public int dame;
    private Animator AnimatorPlayer;
    private int _IDHitPlayer = Animator.StringToHash("ByHitPlayer");
    private int _IDDie = Animator.StringToHash("Die");
   
    private void Awake()
    {
        if (InstancePlayerHealth == null) InstancePlayerHealth = this;
        else Destroy(gameObject); // tránh trùng instance
    }
    private void Start()
    {
        AnimatorPlayer = GetComponent<Animator>();
      
    }
    public void TakeDamage()
    {
        health -= dame;
        AnimatorPlayer.SetTrigger(_IDHitPlayer);
        if (health <= 0)
        { 
            // Tắt di chuyển/điều khiển
           var move = GetComponent<PlayerMovemnet>();
            if (move != null) move.enabled = false;
           var attack = GetComponent<PlayerAttack>();
            if (attack!= null) attack.enabled = false;

            // Nếu có Animator, chơi animation chết
            GetComponent<Animator>()?.SetTrigger("Die");

            // Tắt collider (chặn va chạm)
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Cho Rigidbody chịu trọng lực rơi xuống (nếu chưa chết dưới đất)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                PlayerDie();
            }

            gameObject.layer = LayerMask.NameToLayer("Dead");
            Debug.Log("Lose");
        }
    }

    public void PlayerDie()
    {
        AnimatorPlayer.SetBool(_IDDie, true);
        _input.move = Vector2.zero;
        _input.jump = false;
        StartCoroutine(WaitToDie());
    }
    private IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
    public void CurrentShied()
    {
        shied -= dame;
        Debug.Log($"{shied}");
        if (shied < 0)
        {
            TakeDamage();
        }
    }
   
}
