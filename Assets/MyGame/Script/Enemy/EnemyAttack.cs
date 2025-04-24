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

   

   
    public void EnemyAttacking()
    {
        if (hitPointEnemy != null)
        {
  
                Collider[] hits = Physics.OverlapSphere(hitPointEnemy.position, RadiusPoint, PlayerLayerMask);
                if (hits.Length > 0)
                {
                    PlayerHealth health = hits[0].GetComponent<PlayerHealth>();
                    if (health != null)
                    {
                        health.TakeDamage();
                        Instantiate(effectEnemy, hits[0].transform.position + Vector3.up, Quaternion.identity);
                    }
                }
        }
    }

    public void EnemyAttacking2()
    {
        if (hitPointEnemy2 != null)
        {
   
                Collider[] hits = Physics.OverlapSphere(hitPointEnemy2.position, RadiusPoint, PlayerLayerMask);
                if (hits.Length > 0)
                {
                    PlayerHealth health = hits[0].GetComponent<PlayerHealth>();
                    if (health != null)
                    {
                        health.TakeDamage();
                        Instantiate(effectEnemy, hits[0].transform.position + Vector3.up, Quaternion.identity);
                    }
                }
        }
    }
}
