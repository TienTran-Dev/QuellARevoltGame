using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAi : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent EnemyNavMesh;
    [SerializeField]
    private Transform PlayerTransfrom;
    [SerializeField]
    private Animator EnemyAnimator;
    private int _IDSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        EnemyNavMesh = GetComponent<NavMeshAgent>();
        EnemyAnimator = GetComponent<Animator>();
        PlayerTransfrom = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (PlayerTransfrom != null)
        {
            EnemyNavMesh.SetDestination(PlayerTransfrom.position);
            EnemyAnimator.SetFloat(_IDSpeed, EnemyNavMesh.velocity.magnitude);

        }
        Debug.Log($"{EnemyNavMesh.velocity.magnitude}");
    }
}
