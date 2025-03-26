using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private Animator _AnimatorEnemy;
    [SerializeField]
    private int dameEnemy;
    [SerializeField]
    private int shiedEnemy;
    [SerializeField]
    private Transform[] hitPointEnemy;
    [SerializeField]
    private GameObject effectEnemy;
    [SerializeField]
    private NavMeshAgent findPlayer;
    [SerializeField]
    private LayerMask Player;
    [SerializeField]
    private Transform playTF;
    [SerializeField]
    private float RadiusEnemy;



    private void Start()
    {
        _AnimatorEnemy = GetComponent<Animator>();
        effectEnemy = GetComponent<GameObject>();
    }


    private void EnemyAttacking()
    {
        if (_AnimatorEnemy != null)
        {
            
            foreach (Transform item in hitPointEnemy)
            {
                Collider[] HitPoint = Physics.OverlapSphere(item.transform.position, RadiusEnemy, Player);
                if (HitPoint.Length > 0)
                {
                    
                    GetComponent<PlayerHealth>().TakeDamage();
                }
            }
        }

    }
}
