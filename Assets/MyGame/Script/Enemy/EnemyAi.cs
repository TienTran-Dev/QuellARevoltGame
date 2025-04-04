using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.WSA;


public class EnemyAi : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent EnemyNavMesh;
    [SerializeField]
    private Transform PlayerTransfrom;
    private Animator EnemyAnimator;
    private int _IDSpeed = Animator.StringToHash("Speed");
    private int _IDAttack = Animator.StringToHash("AttackEnemy");
    private int currentWayPoint = 0;
    [SerializeField]
    private Transform WayPoint;
    public float distance;
    public float _valueSpeed;
    [SerializeField]
    private float ValueSprint;
    [SerializeField]
    private float ValueWalk;
    [SerializeField]
    private GameObject Player;
    public static EnemyAttack _EnemyAttack;

   


    private void Awake()
    {
        EnemyNavMesh = GetComponent<NavMeshAgent>();
        EnemyAnimator = GetComponent<Animator>();
        PlayerTransfrom = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Player = GetComponent<GameObject>();
        WayPoint.SetParent(null);
        if (EnemyNavMesh.SetDestination(WayPoint.GetChild(currentWayPoint).position))
        {
            _valueSpeed = 1f;
        }
       
        


    }
    
    private void Update()
    {
        distance = Vector3.Distance(PlayerTransfrom.position, transform.position);
        if (distance <= 10)
        {
            MoveToPlayer();
        }
        else
        {
            EmemyRadius();
        }
        
    }

    public void EmemyRadius()
    {
        if (EnemyNavMesh.remainingDistance <= 0.2f)
        {
            currentWayPoint++;
            if (currentWayPoint >= WayPoint.childCount)
            {
                currentWayPoint = 0;
            }
            EnemyNavMesh.SetDestination(WayPoint.GetChild(currentWayPoint).position);
            EnemyNavMesh.speed = ValueWalk;
            EnemyAnimator.SetFloat(_IDSpeed, _valueSpeed=ValueWalk);
          
          
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _EnemyAttack.DistanceAttack();
        }
    }

    private void MoveToPlayer()
    {
        if (PlayerTransfrom != null)
        {
            // Kiểm tra nếu enemy ở gần player
            float distance = Vector3.Distance(transform.position, PlayerTransfrom.position);
            
            if (distance <= 1f) // khoảng cách có thể điều chỉnh
            {
                EnemyNavMesh.isStopped = true;
                EnemyAnimator.SetFloat(_IDSpeed, _valueSpeed);
                EnemyAnimator.SetTrigger(_IDAttack);
                
            }
            else
            {
                EnemyNavMesh.isStopped = false;
                EnemyNavMesh.SetDestination(PlayerTransfrom.position);
                EnemyNavMesh.speed = ValueSprint;
                EnemyAnimator.SetFloat(_IDSpeed, _valueSpeed = ValueSprint);

            }
        }
    }

}
