using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
  
    [SerializeField]
    private Transform hitPointEnemy; 
    [SerializeField]
    private Transform hitPointEnemy2;
    [SerializeField]
    private GameObject effectEnemy;
    [SerializeField]
    private LayerMask PlayerLayerMask;
    [SerializeField]
    private float RadiusPoint;
    [SerializeField]
    private Transform player;
    private static EnemyAi _EnemyAi;
   

    private void Start()
    {
        effectEnemy = GetComponent<GameObject>();
        _EnemyAi = GetComponent<EnemyAi>();
       
    }

   


    public void DistanceAttack()
    {

        if (_EnemyAi.distance <= 10)
        {
            if (_EnemyAi.distance < 1f)
            {
                EnemyAttacking();
                EnemyAttacking2();

            }
        }
       
    }
   

    public void EnemyAttacking()
    {
        if (hitPointEnemy != null)
        {
           
                Collider[] HitPoint = Physics.OverlapSphere(hitPointEnemy.position, RadiusPoint, PlayerLayerMask);
                if (HitPoint.Length > 0)
                { 
                   HitPoint[0].GetComponent<PlayerHealth>().TakeDamage();
                    Instantiate(effectEnemy.transform, HitPoint[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); 
                }


            
        }

    } public void EnemyAttacking2()
    {
        if (hitPointEnemy2 != null)
        {
           
                Collider[] HitPoint = Physics.OverlapSphere(hitPointEnemy2.position, RadiusPoint, PlayerLayerMask);
                if (HitPoint.Length > 0)
                { 
                   HitPoint[0].GetComponent<PlayerHealth>().TakeDamage();
                    Instantiate(effectEnemy.transform, HitPoint[0].transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity); 
                }


            
        }

    }
}
