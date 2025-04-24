using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Windows;
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
    public static EnemyAttack _EnemyAttack;
    public static PlayerHealth _PlayerHealth;
   


    private void Awake()
    {
        EnemyNavMesh = GetComponent<NavMeshAgent>();
        EnemyAnimator = GetComponent<Animator>();
        PlayerTransfrom = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        WayPoint.SetParent(null);
        EnemyNavMesh.SetDestination(WayPoint.GetChild(currentWayPoint).position);
        _valueSpeed = ValueWalk;
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
            EnemyRadius();
        }
        
    }

    public void EnemyRadius()
    {
        if (EnemyNavMesh.remainingDistance <= 0.2f)
        {
            int newWaypoint;
            do
            {
                newWaypoint = Random.Range(0, WayPoint.childCount);
            } while (newWaypoint == currentWayPoint && WayPoint.childCount > 1);

            currentWayPoint = newWaypoint;

            Vector3 nextPos = WayPoint.GetChild(currentWayPoint).position;
            EnemyNavMesh.SetDestination(nextPos);
            //Cập nhật waypoint mới và di chuyển đến vị trí đó
            _valueSpeed = ValueWalk;
            EnemyNavMesh.speed = _valueSpeed;
            EnemyAnimator.SetFloat(_IDSpeed, _valueSpeed);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
            EnemyAnimator.SetTrigger(_IDAttack);
        
        
    }

    private void MoveToPlayer()
    {
        if (PlayerTransfrom != null)  // Nếu không có player  → bỏ qua
        {
            float distance = Vector3.Distance(transform.position, PlayerTransfrom.position); // Tính khoảng cách với player

        }

        // Nếu player đã chết
        if (PlayerHealth.InstancePlayerHealth.health <= 0)
        {
            EnemyNavMesh.isStopped = false;
            EnemyRadius(); // Player chết → quay lại tuần tra
            return;
        }

        if (distance <= 1.4f) // Nếu ở gần player (có thể tấn công)
        {
            EnemyNavMesh.isStopped = true; // Dừng lại
            EnemyNavMesh.speed = 0f;
            EnemyAnimator.SetFloat(_IDSpeed, _valueSpeed=0f); // Gửi tốc độ = 0 để idle
                EnemyAnimator.SetTrigger(_IDAttack); // Gọi animation tấn công
            
        }
        else if (distance > 2f && distance <= 10f) // Nếu đang ở khoảng vừa đủ để đuổi
        {
            EnemyNavMesh.isStopped = false;
            EnemyNavMesh.SetDestination(PlayerTransfrom.position); // Đuổi theo player
            EnemyNavMesh.speed = ValueSprint;
            EnemyAnimator.SetFloat(_IDSpeed, _valueSpeed = ValueSprint); // Cập nhật animation chạy
        }
    }


}
