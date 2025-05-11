using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public int shied;
    public int dame;
    private Animator _Animator;
    private int _IDHit = Animator.StringToHash("ByHit");
    private int _IDEnemyDie = Animator.StringToHash("EnemyDie");
    private Vector3 attackerPosition;

    private void Start()
    {
        _Animator = GetComponent<Animator>();
    }
    public void TakeDamage()
    {
            health -= dame;
        _Animator.SetTrigger(_IDHit);
        AnimatorStateInfo stateInfo = _Animator.GetCurrentAnimatorStateInfo(0);

        // Đẩy lùi enemy khỏi hướng player
        Vector3 knockbackDir = (transform.position - attackerPosition).normalized;
        float knockbackDistance = 1f; // có thể điều chỉnh
        transform.position += knockbackDir * knockbackDistance;

        if (stateInfo.IsTag("Die")) return;
        if (health <= 0)
        {
           
            _Animator.SetBool(_IDEnemyDie, true);
            if (EnemyController.Instance != null)
            {
                EnemyController.Instance.CountEnemyDead();
            }
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            var EnemyAI =  GetComponent<EnemyAi>();
            if (EnemyAI != null) EnemyAI.enabled = false;
            var ATE = GetComponent<EnemyAttack>();
            if (ATE != null) ATE.enabled = false; 
            gameObject.layer = LayerMask.NameToLayer("Dead");
            StartCoroutine(WaitToDieEnemy());
        } 
       
    }

    private IEnumerator WaitToDieEnemy()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
    }
    public void CurrentShied()
    {
        shied -= dame;
        
        if (shied < 0)
        {
            health -= dame;
        }
    }
}
