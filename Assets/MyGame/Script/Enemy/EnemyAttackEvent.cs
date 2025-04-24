using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    private EnemyAttack _EnemyEvent;

    private void Start()
    {
        _EnemyEvent = GetComponent<EnemyAttack>();
    }

    private void AttackEventEnemy()
    {
        _EnemyEvent.EnemyAttacking();   
    }
    
    private void AttackEventEnemy2()
    {
        _EnemyEvent.EnemyAttacking2();
    }
}
